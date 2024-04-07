using Microsoft.EntityFrameworkCore;
using Udemy.Core.DTOs;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.DTOs.CoursePartsDtos;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.Repository;
namespace Udemy.EF.Repository
{


    public class CourseDataRepository : BaseRepository<UdemyContext>, ICourseDataRepository
    {
        private readonly UdemyContext _dbcontext;
        private readonly IBaseRepository<Course> _courseDataRepository;

        public CourseDataRepository(IBaseRepository<Course> courseDataRepository, UdemyContext dbcontext) : base(dbcontext)
        {
            _dbcontext = dbcontext;
            _courseDataRepository = courseDataRepository;
        }



        public async Task<CourseSectionsDto> GetSectionsByCourseIdAsync(int courseId, string userId)
        {
            var sections = await _dbcontext.Sections
                .Include(s => s.Lessons).ThenInclude(l => l.Notes)
                .Include(s => s.Lessons).ThenInclude(l => l.LessonProgress)
                .Where(s => s.CourseID == courseId)
                .ToListAsync();

            var courseSectionsDto = new CourseSectionsDto
            {
                Sections = sections.Select(section => MapSectionDto(section, userId)).ToList()
            };

            return courseSectionsDto;
        }

        public async Task<IEnumerable<AnnouncmentDto>> GetAnnouncementByCourseIdAsync(int courseId)
        {
            var announcements = await _dbcontext.Announcements
                .Include(a => a.Course)
                .ThenInclude(c => c.Instructor)
                .Where(s => s.CourseID == courseId)
                .ToListAsync();

            var courseAnnoncementDto = announcements.Select(a => new AnnouncmentDto
            {
                Id = a.AnnouncementID,
                Content = a.Content,
                InstructorName = $"{a.Course.Instructor.FirstName ?? "Unknown"} {a.Course.Instructor.LastName ?? "Unknown"}",
                InstructorImage = a.Course.Instructor.Image,
                Date = a.DatePosted.ToString("d/M/yyyy") // Format date as "d/m/y"
            });

            return courseAnnoncementDto;
        }

        public async Task<IEnumerable<FeedbackDto>> GetReviewsByCourseIdAsync(int courseId)
        {
            var courseReviews = await _dbcontext.Feedbacks
        .Include(f => f.Enrollment.User)
        .Where(f => f.Enrollment.CourseId == courseId)
        .Select(f => new FeedbackDto
        {
            Rate = f.Rate,
            ReviewComment = f.Comment,
            StudentName = $"{f.Enrollment.User.FirstName ?? "Unknown"} {f.Enrollment.User.LastName ?? "Unknown"}",
            StudentImage = f.Enrollment.User.Image ?? "",
            Date = f.Date.ToString("d/M/yyyy") // Format date as "d/m/y"
        })
        .ToListAsync();

            return courseReviews;
        }

        public async Task<FeedbackDto> GetStudentReviewOnCourseByCourseIdAsync(int courseId, string userId)
        {
            var feedback = await _dbcontext.Feedbacks
                .Include(f => f.Enrollment)
                .ThenInclude(e => e.User)
                .FirstOrDefaultAsync(f => f.Enrollment.UserId == userId && f.Enrollment.CourseId == courseId);

            if (feedback == null)
            {
                return new FeedbackDto
                {
                    Rate = 0,
                    ReviewComment = "",
                    StudentName = "",
                    StudentImage = "",
                    Date = "" // Format date as "d/m/y"
                }; ; // Return null or handle the case where feedback is not found
            }

            var studentReview = new FeedbackDto
            {
                Rate = feedback.Rate,
                ReviewComment = feedback.Comment ?? "",
                StudentName = $"{feedback.Enrollment.User.UserName ?? "Unknown"}",
                StudentImage = feedback.Enrollment.User.Image ?? "",
                Date = feedback.Date.ToString("d/M/yyyy")// Format date as "d/m/y"
            };

            return studentReview;
        }

        public async Task<bool> SetStudentReviewOnCourse(int courseId, string userId, FeedbackDto feedbackDto)
        {
            // Find the enrollment record for the specified course and user
            var enrollment = await _dbcontext.Enrollments
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
            {
                // If the enrollment record doesn't exist, return false indicating failure
                return false;
            }

            // Check if there's already a feedback record for this enrollment
            var existingFeedback = await _dbcontext.Feedbacks
                .FirstOrDefaultAsync(f => f.EnrollmentId == enrollment.Id);

            // If there's no existing feedback, create a new feedback record
            if (existingFeedback == null)
            {
                // Create a new feedback record
                existingFeedback = new Feedback
                {
                    EnrollmentId = enrollment.Id // Set the EnrollmentId
                };

                // Update the feedback properties
                existingFeedback.Rate = feedbackDto.Rate;
                existingFeedback.Comment = feedbackDto.ReviewComment;
                existingFeedback.Date = DateTime.UtcNow; // Assuming you want to set the current date/time

                // Add the new feedback record to the DbContext
                _dbcontext.Feedbacks.Add(existingFeedback);
            }
            else
            {
                // Update the existing feedback properties
                existingFeedback.Rate = feedbackDto.Rate;
                existingFeedback.Comment = feedbackDto.ReviewComment;
                existingFeedback.Date = DateTime.UtcNow; // Assuming you want to update the date/time
            }

            // Save changes to the database
            await _dbcontext.SaveChangesAsync();

            // Return true to indicate success
            return true;
        }


        public async Task<IEnumerable<CourseCommentDto>> GetCommentsByCourseIdAsync(int courseId, string userId)
        {
            var courseComments = await _dbcontext.Comments
                .Include(c => c.Enrollment)
                .ThenInclude(e => e.User)
                .Where(c => c.Enrollment.CourseId == courseId)
                .ToListAsync();

            var commentsDto = courseComments.Select(c => new CourseCommentDto
            {
                Id = c.CommentID,
                Content = c.Content,
                StudentName = $"{c.Enrollment.User.FirstName ?? "Unknown"} {c.Enrollment.User.LastName ?? "Unknown"}",
                StudentImage = c.Enrollment.User.Image ?? "",
                AnswerTo = c.AnswerTo,
                isReply = c.AnswerTo != null,
                isUserComment = c.Enrollment.UserId == userId,
                CourseID = c.CourseID,
                isUpdated = c.isUpdated,
                Date = c.Date.ToString("d/M/yyyy")


            });

            return commentsDto;
        }

        public async Task<bool> SetStudentCommentOnCourse(int courseId, string userId, CourseCommentDto commentDto)
        {
            try
            {
                // Check if the user is enrolled in the course
                var enrollment = await _dbcontext.Enrollments.FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);
                if (enrollment == null)
                {
                    return false; // Enrollment not found
                }

                Comment commentToUpdate = null;

                if (commentDto.Id > 0)
                {
                    // Retrieve existing comment
                    commentToUpdate = await _dbcontext.Comments.FindAsync(commentDto.Id);

                    if (commentToUpdate == null)
                    {
                        return false; // Comment not found
                    }

                    // Update existing comment properties
                    commentToUpdate.Content = commentDto.Content;
                    commentToUpdate.AnswerTo = commentDto.AnswerTo;
                    commentToUpdate.isUpdated = true; // Set isUpdated to true for updated comments
                    commentToUpdate.Date = DateTime.UtcNow;
                }
                else
                {
                    // Create new comment
                    var newComment = new Comment
                    {
                        Content = commentDto.Content,
                        EnrollmentID = enrollment.Id,
                        CourseID = courseId,
                        AnswerTo = commentDto.AnswerTo,
                        Date = DateTime.UtcNow,
                        isReply = commentDto.isReply,
                        isUpdated = false // Set isUpdated to false for new comments
                    };

                    _dbcontext.Comments.Add(newComment);
                }

                await _dbcontext.SaveChangesAsync();

                return true; // Successfully added or updated the comment
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return false; // Failed to add or update the comment
            }
        }

        public async Task<OverviewDto> GetCourseOverViewById(int courseId)
        {
            try
            {
                // Retrieve course details including related entities
                var course = await _dbcontext.Courses
                    .Include(c => c.Objectives)
                    .Include(c => c.Requirements)
                    .Include(c => c.Sections).ThenInclude(s => s.Lessons)
                    .Include(c => c.Instructor).ThenInclude(i => i.CreatedCourses).ThenInclude(c => c.Enrollments).ThenInclude(e => e.Feedback)
                    .FirstOrDefaultAsync(c => c.CourseID == courseId);

                if (course == null)
                {
                    // Log error or throw specific exception for course not found
                    return null; // Course not found
                }

                // Retrieve instructor details from the course
                var instructor = course.Instructor;
                if (instructor == null)
                {
                    // Log error or throw specific exception for instructor not found
                    return null; // Instructor not found
                }

                // Calculate the instructor rate (average feedback rate from created courses)
                var instructorRate = instructor.CreatedCourses
                    .SelectMany(c => c.Enrollments)
                    .Select(e => e.Feedback?.Rate)
                    .Average() ?? 0;

                // Initialize the overview DTO with course and instructor details
                var overviewDto = new OverviewDto
                {
                    CourseID = courseId,
                    BriefDescription = course.BriefDescription,
                    FullDescription = course.FullDescription,
                    Level = course.Level,
                    Language = course.Language,
                    InstructorName = $"{instructor.FirstName ?? "Unknown"} {instructor.LastName ?? "Unknown"}",
                    InstrucHeadlind = instructor.Headline ?? "",
                    InstructorBiography = instructor.Biography ?? "",
                    InstructorRate = instructorRate,
                    InstructorImage = instructor.Image ?? ""
                };

                // Retrieve course objectives and requirements
                overviewDto.Objectives = course.Objectives.Select(o => new ObjectiveDto { Content = o.Description }).ToList();
                overviewDto.Requirements = course.Requirements.Select(r => new RequirementDto { Content = r.Description }).ToList();

                // Calculate and assign total number of students enrolled in the course
                overviewDto.NumStudent = await _dbcontext.Enrollments.CountAsync(e => e.CourseId == courseId);

                // Calculate and assign total number of lessons in the course
                overviewDto.TotalLessons = await _dbcontext.Lessons.CountAsync(l => l.Section.CourseID == courseId);

                // Calculate and assign total time in minutes for all lessons in the course
                overviewDto.TotalHours = FormatTotalHours(await _dbcontext.Lessons
                    .Where(l => l.Section.CourseID == courseId)
                    .SumAsync(l => l.Duration));

                return overviewDto;
            }
            catch (Exception ex)
            {
                // Log the exception or rethrow for centralized error handling
                return null; // Return null in case of an error
            }
        }

        public async Task<SingleCourseDto> GetSingleCourse(int courseId)
        {
            var course = await _dbcontext.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments).ThenInclude(e => e.Feedback).ThenInclude(f => f.Rate)
                .Include(c => c.Sections).ThenInclude(s => s.Lessons)
                .Include(c => c.Category)
                .ThenInclude(cat => cat.ParentCategory).ThenInclude(cat => cat.ParentCategory) // Include the parent category
                .FirstOrDefaultAsync(c => c.CourseID == courseId);

            if (course == null)
                return null; // Or handle accordingly

            var totalMinutes = course.Sections?.Sum(s => s.Lessons.Sum(l => l.Duration)) ?? 0;

            var hours = totalMinutes / 60;
            var minutes = totalMinutes % 60;
            var totalMinutesFormatted = $"{hours}h {minutes}m";

            var totalLessons = course.Sections?.Sum(s => s.Lessons.Count) ?? 0;
            var totalSections = course.Sections?.Count() ?? 0;
            var rate = course.Enrollments?.Average(e => e.Feedback?.Rate ?? 0) ?? 0;
            var numFeedback = course.Enrollments?.Count(e => e.Feedback != null) ?? 0;
            var numStudents = course.Enrollments?.Count() ?? 0;
            var lastUpdateDate = course.DateUpdated.ToString("dd/MM/yyyy");

            var singleCourseDto = new SingleCourseDto
            {
                ID = course.CourseID,
                Name = course.Name,
                BriefDescription = course.BriefDescription,
                Category = course.Category?.ParentCategory?.ParentCategory?.Name ?? "",
                SubCategoey = course.Category?.ParentCategory?.Name ?? "",
                Topic = course.Category?.Name ?? "",
                IntroVideo = course.DescVideo ?? "",
                Price = course.Price,
                TotalMinutes = totalMinutesFormatted,
                TotalLessons = totalLessons,
                TotalSections = totalSections,
                Rate = rate,
                NumFeedback = numFeedback,
                NumStudents = numStudents,
                LastUpdateDate = lastUpdateDate,
                FullDescribtion = course.FullDescription ?? ""
                // Add other properties as needed
            };

            return singleCourseDto;
        }

        public async Task<CourseInstructorDto> GetCourseInstructor(int courseId)
        {
            var instructorDto = await _dbcontext.Courses
                .Include(c => c.Instructor)
                    .ThenInclude(i => i.CreatedCourses)
                        .ThenInclude(c => c.Enrollments)
                            .ThenInclude(e => e.Feedback)
                .Where(c => c.CourseID == courseId)
                .Select(c => new CourseInstructorDto
                {
                    InstructorId = c.InstructorID,
                    Name = $"{c.Instructor.FirstName ?? "Unknown"} {c.Instructor.LastName ?? "Unknown"}",
                    Image = c.Instructor.Image ?? "",
                    HeadLine = c.Instructor.Headline ?? "",
                    Boiography = c.Instructor.Biography ?? "",
                    Rate = c.Instructor.CreatedCourses
                                .SelectMany(cc => cc.Enrollments)
                                .Where(e => e.Feedback != null)
                                .Select(e => e.Feedback.Rate)
                                .Average(),
                    CoursesCount = c.Instructor.CreatedCourses.Count,
                    Courses = c.Instructor.CreatedCourses.Select(c => new CourseWithObjectivesDto
                    {
                        ID = c.CourseID,
                        Image = c.Cover,
                        Name = c.Name,
                        InstructorName = c.Instructor.FirstName ?? "Unknown" + " " + c.Instructor.FirstName ?? "Unknown",
                        Rate = CalculateAverageRate(c),
                        Price = c.Price,
                        ReviewersNumber = c.Enrollments.Count(e => e.Feedback != null) != 0 ? c.Enrollments.Count(e => e.Feedback != null) : 0,
                        Objectives = c.Objectives.Select(o => new ObjectiveDto
                        {
                            ID = o.ObjectiveID,
                            Content = o.Description
                        }

                )


                    }) ?? Enumerable.Empty<CourseWithObjectivesDto>(),
                    StudentsCount = c.Instructor.CreatedCourses.SelectMany(cc => cc.Enrollments)
                                        .Select(e => e.UserId)
                                        .Distinct()
                                        .Count(),
                    FeedbacksCount = c.Instructor.CreatedCourses
                                        .SelectMany(cc => cc.Enrollments)
                                        .Count(e => e.Feedback != null),
                })
                .FirstOrDefaultAsync();

            // Handle case when the course or instructor is not found
            if (instructorDto == null)
            {
                // You might throw an exception or return appropriate response
                return null;
            }

            return instructorDto;
        }

        public async Task<CourseSectionsDto> GetCourseSections(int courseId)
        {
            var sectionsDto = await _dbcontext.Sections
                .Include(s => s.Lessons)
                .ThenInclude(l => l.LessonProgress)
                .Where(s => s.CourseID == courseId)
                .Select(s => new SectionDto
                {
                    SectionId = s.SectionID,
                    SectionName = s.Title,
                    TotalLessons = s.Lessons.Count,
                    TotalMinutes = FormatTotalHours(s.Lessons.Sum(l => l.Duration)),
                    Lessons = s.Lessons.Select(l => new LessonDto
                    {
                        LessonId = l.LessonID,
                        LessonName = l.Title,
                        LessonTimeInMinutes = l.Duration,
                        LessonVideo = l.File,
                        IsCompleted = l.LessonProgress.IsCompleted,
                        Notes = null
                    }).ToList()
                })
                .ToListAsync();

            if (sectionsDto == null)
            {
                return null; // Or handle accordingly
            }

            return new CourseSectionsDto
            {
                Sections = sectionsDto
            };
        }

        public async Task<CourseRelatedDto> GetRelatedCourseData(int courseId)
        {
            // Retrieve featured feedback
            var featuredFeedback = await _dbcontext.Feedbacks
                .Include(f => f.Enrollment)
                    .ThenInclude(e => e.User)
                .Where(f => f.Enrollment.CourseId == courseId)
                .OrderByDescending(f => f.Date)
                .Select(f => new FeedbackDto
                {
                    Rate = f.Rate,
                    ReviewComment = f.Comment,
                    StudentName = $"{f.Enrollment.User.FirstName ?? "Unknown"} {f.Enrollment.User.LastName ?? "Unknown"}",
                    StudentImage = f.Enrollment.User.Image ?? "",
                    Date = f.Date.ToString("d/M/yyyy") // Format date as "d/m/y"
                })
                .FirstOrDefaultAsync();



            // Retrieve objectives related to the course
            var objectives = await _dbcontext.Objectives
                .Where(o => o.CourseID == courseId)
                .Select(o => new ObjectiveDto
                {
                    ID = o.ObjectiveID,
                    Content = o.Description
                })
                .ToListAsync();

            // Retrieve requirements related to the course
            var requirements = await _dbcontext.Requirements
                .Where(r => r.CourseID == courseId)
                .Select(r => new RequirementDto
                {
                    Id = r.RequirementID,
                    Content = r.Description
                })
                .ToListAsync();

            if (featuredFeedback == null && !objectives.Any() && !requirements.Any())
            {
                return null; // Or handle accordingly if no related data found
            }

            return new CourseRelatedDto
            {
                FeaturedFeedback = featuredFeedback,
                Objectives = objectives,
                Requirements = requirements
            };
        }

        public async Task<bool> SetStudentCourseLessonsStatus(int courseId, string userId, IEnumerable<LassonStatusDto> lessonStatusDto)
        {
            var lessonProgressList = await _dbcontext.LessonProgresses
                .Include(lp => lp.Enrollment)
                .Where(lp => lp.Enrollment.UserId == userId && lp.Enrollment.CourseId == courseId)
                .ToListAsync();

            foreach (var lessonDto in lessonStatusDto)
            {
                var lessonProgress = lessonProgressList.FirstOrDefault(lp => lp.LessonID == lessonDto.LessonId);
                if (lessonProgress != null)
                {
                    lessonProgress.IsCompleted = lessonDto.IsCompleted;
                }
                else
                {
                    // Handle case where LessonProgress doesn't exist for the lesson, maybe log or throw exception
                }
            }

            try
            {
                await _dbcontext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Handle exception, log it or return false indicating failure
                return false;
            }
        }

        public async Task<CertificateDto> GetOrCreateCertificateDataByUserIdAndCourseId(string userId, int courseId)
        {
            var certificateData = await _dbcontext.Certificates
                .Include(c => c.Enrollment)
                    .ThenInclude(e => e.User)
                .Include(c => c.Enrollment)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(c => c.Enrollment.UserId == userId && c.Enrollment.CourseId == courseId);

            if (certificateData == null)
            {
                certificateData = await CreateCertificateEntry(userId, courseId);
            }

            if (certificateData != null)
            {
                var studentName = string.IsNullOrWhiteSpace(certificateData.Enrollment.User.FirstName) && string.IsNullOrWhiteSpace(certificateData.Enrollment.User.LastName)
                    ? certificateData.Enrollment.User.UserName
                    : certificateData.Enrollment.User.FirstName + " " + certificateData.Enrollment.User.LastName;

                var course = certificateData.Enrollment.Course;
                float averageRate = await _dbcontext.Feedbacks
                    .Where(f => f.Enrollment.CourseId == courseId)
                    .AverageAsync(f => f.Rate);

                var certificateDto = new CertificateDto
                {
                    StudentName = studentName,
                    StudentImage = certificateData.Enrollment.User.Image, // Assuming there's a property for user image URL
                    CourseName = course.Name,
                    InstructorName = course.Instructor.FirstName + " " + course.Instructor.LastName,
                    Rate = averageRate,
                    Date = certificateData.Enrollment.EnrollmentDate.ToShortDateString(), // Assuming EnrollmentDate is a property of Enrollment
                    TotalLectures = await _dbcontext.Lessons.CountAsync(l => l.Section.CourseID == courseId),
                    TotalTime = await FormatTotalTime(courseId)
                };

                return certificateDto;
            }

            return null;
        }












        #region private methods

        private async Task<Certificate> CreateCertificateEntry(string userId, int courseId)
        {
            var enrollment = await _dbcontext.Enrollments
                .Include(e => e.Course)
                    .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(e => e.UserId == userId && e.CourseId == courseId);

            if (enrollment == null)
                return null;

            var newCertificate = new Certificate
            {
                EnrollmentID = enrollment.Id,
                                                           
            };

            _dbcontext.Certificates.Add(newCertificate);
            await _dbcontext.SaveChangesAsync();

            return newCertificate;
        }

        private SectionDto MapSectionDto(Section section, string userId)
        {
            return new SectionDto
            {
                SectionId = section.SectionID,
                SectionName = section.Title,
                Lessons = section.Lessons.Select(lesson => MapLessonDto(lesson, userId)).ToList()
            };
        }

        private LessonDto MapLessonDto(Lesson lesson, string userId)
        {
            var isCompleted = _dbcontext.LessonProgresses
                .Include(p => p.Enrollment)
                .FirstOrDefault(p => p.LessonID == lesson.LessonID && p.Enrollment.UserId == userId);

            var notes = _dbcontext.Notes
                .Include(n => n.Enrollment)
                .Where(n => n.LessonID == lesson.LessonID && n.Enrollment.UserId == userId)
                .Select(MapNoteDto)
                .ToList();

            return new LessonDto
            {
                LessonId = lesson.LessonID,
                LessonName = lesson.Title,
                LessonTimeInMinutes = lesson.Duration,
                LessonVideo = lesson.File,
                IsCompleted = isCompleted.IsCompleted,
                Notes = notes
            };
        }

        private async Task<string> FormatTotalTime(int courseId)
        {
            int totalMinutes = await _dbcontext.Lessons
                .Where(l => l.Section.CourseID == courseId)
                .SumAsync(l => l.Duration);

            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;

            return $"{hours}h {minutes}m";
        }
        private NoteDto MapNoteDto(Note note)
        {
            return new NoteDto
            {
                Id = note.NoteId,
                Content = note.Content
            };
        }
        private float CalculateAverageRate(Course course)
        {
            if (course.Enrollments == null || course.Enrollments.Count == 0)
                return 0;

            return course.Enrollments.Average(enrollment => enrollment.Feedback?.Rate ?? 0);

        }

        string FormatTotalHours(int totalMinutes)
        {
            int hours = totalMinutes / 60;
            int minutes = totalMinutes % 60;
            return $"{hours}h {minutes}m";
        }

        #endregion
    }


}
