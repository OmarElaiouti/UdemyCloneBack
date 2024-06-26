﻿//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Hosting;
//using System.Security.Claims;
//using Udemy.BLL.Services.Interfaces;
//using Udemy.DAL.Context;
//using Udemy.DAL.DTOs;
//using Udemy.DAL.Interfaces;
//using static System.Net.Mime.MediaTypeNames;

//namespace UdemyApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class InstructorDashboardController : ControllerBase
//    {
//        private readonly ICourseDataService _courseDataService;
//        private readonly IAuthService _authService;
//        private readonly IWebHostEnvironment _hostEnvironment;
//        private readonly UdemyContext _dbcontext;

//        public InstructorDashboardController(UdemyContext dbcontext,ICourseDataService courseDataService, IWebHostEnvironment hostEnvironment, IAuthService authService)
//        {
//            _courseDataService = courseDataService;
//            _authService = authService;
//            _hostEnvironment = hostEnvironment;
//            _dbcontext = dbcontext;
//        }

//        [HttpPost("create-or-update-course")]
//        public async Task<ActionResult<bool>> CreateOrUpdateCourse([FromBody] InstructorDashboardDataToPostDto courseData, [FromForm(Name = "lessonFiles")] List<string> lessonFiles, [FromForm(Name = "coverImage")] string coverImage)
//        {
//            try
//            {
//                string? instructorId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

//                if (string.IsNullOrEmpty(instructorId))
//                {
//                    return BadRequest("Invalid token or token expired.");
//                }

//                // Check if the course already exists
//                var existingCourse = await _dbcontext.Courses.FirstOrDefaultAsync(c => c.Name == courseData.CourseTitle);

//                if (existingCourse != null)
//                {
//                    // Update existing course
//                    UpdateCourse(existingCourse, courseData, coverImage);
//                }
//                else
//                {
//                    // Create new course
//                    existingCourse = CreateCourse(courseData, instructorId, coverImage);
//                    _dbcontext.Courses.Add(existingCourse);
//                }

//                // Create or update course requirements, objectives, sections, and lessons
//                await CreateOrUpdateCourseRequirements(existingCourse, courseData.Requirements);
//                await CreateOrUpdateCourseObjectives(existingCourse, courseData.Objectives);
//                await CreateOrUpdateCourseSections(existingCourse, courseData.Section, lessonFiles);

//                // Save changes to the database
//                await _dbcontext.SaveChangesAsync();

//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                //_logger.LogError(ex, "An error occurred while creating or updating a course.");
//                return StatusCode(500, "An error occurred while processing your request.");
//            }
//        }
        
        
//        [HttpGet("course/{courseId}")]
//        public async Task<ActionResult<InstructorDashboardDataToGetDto>> GetCourseData(int courseId)
//        {
//            try
//            {
//                var course = await _dbcontext.Courses.Include(c => c.Requirements)
//                                                      .Include(c => c.Objectives)
//                                                      .Include(c => c.Sections)
//                                                      .ThenInclude(s => s.Lessons)
//                                                      .FirstOrDefaultAsync(c => c.CourseID == courseId);

//                if (course == null)
//                {
//                    return NotFound("Course not found.");
//                }

//                // Map course entity to DTO
//                var courseData = new InstructorDashboardDataToGetDto
//                {
//                    CourseTitle = course.Name,
//                    Price = course.Price,
//                    Level = course.Level,
//                    Language = course.Language,
//                    CourseDisc = course.BriefDescription,
//                    FullDescription = course.FullDescription,
//                    CategoryName = course.Category?.Name,
//                    Cover = course.Cover, // Assuming the cover is directly accessible via URL
//                    Requirements = course.Requirements.Select(r => new RequirementDto { Content = r.Description }),
//                    Objectives = course.Objectives.Select(o => new ObjectiveDto { Content = o.Description }),
//                    Sections = course.Sections.Select(s => new SectionWithoutFile
//                    {
//                        SectionName = s.Title,
//                        Lessons = s.Lessons.Select(l => new LessonWithoutFileDto
//                        {
//                            LessonName = l.Title,
//                            LessonTimeInMinutes = l.Duration
//                        })
//                    })
//                };

//                return Ok(courseData);
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                //_logger.LogError(ex, "An error occurred while retrieving course data.");
//                return StatusCode(500, "An error occurred while processing your request.");
//            }
//        }

//        [HttpGet("check-instructor-profile")]
//        public async Task<ActionResult<bool>> CheckInstructorProfile([FromHeader(Name = "Authorization")] string token)
//        {
//            try
//            {
//                string instructorId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

//                if (string.IsNullOrEmpty(instructorId))
//                {
//                    return BadRequest("Invalid token or token expired.");
//                }

//                var instructor = await _dbcontext.Users.FindAsync(instructorId);

//                if (instructor == null)
//                {
//                    return NotFound("Instructor not found.");
//                }

//                // Check if the instructor has first name, last name, biography, and headline
//                bool hasProfile = !string.IsNullOrEmpty(instructor.FirstName) &&
//                                  !string.IsNullOrEmpty(instructor.LastName) &&
//                                  !string.IsNullOrEmpty(instructor.Biography) &&
//                                  !string.IsNullOrEmpty(instructor.Headline);

//                return Ok(hasProfile);
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                //_logger.LogError(ex, "An error occurred while checking instructor profile.");
//                return StatusCode(500, "An error occurred while processing your request.");
//            }
//        }

//        [HttpGet("instructor-courses")]
//        public async Task<ActionResult<IEnumerable<CourseForDashboardDto>>> GetInstructorCourses([FromHeader(Name = "Authorization")] string token)
//        {
//            try
//            {
//                string instructorId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

//                if (string.IsNullOrEmpty(instructorId))
//                {
//                    return BadRequest("Invalid token or token expired.");
//                }

//                // Retrieve courses associated with the instructor along with their objectives
//                var coursesWithObjectives = await _dbcontext.Courses
//                    .Include(c=>c.Enrollments)
//                    .ThenInclude(e=>e.Feedback)
//                    .Where(c => c.InstructorID == instructorId)
//                    .Select(c => new CourseForDashboardDto
//                    {
//                        ID = c.CourseID,
//                        Name = c.Name,
//                        InstructorName = c.Instructor.FirstName + " " + c.Instructor.LastName,
//                        Price = c.Price,
//                        Rate = c.Enrollments.Any() ? c.Enrollments.Average(e => e.Feedback != null ? e.Feedback.Rate : 0) : 0,
//                        ReviewersNumber = c.Enrollments.Count(e => e.Feedback != null),
//                        BriefDescription = c.BriefDescription,
//                        Image = c.Cover??""
//                    })
//                    .ToListAsync();

//                return Ok(coursesWithObjectives);
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                //_logger.LogError(ex, "An error occurred while retrieving instructor courses with objectives.");
//                return StatusCode(500, "An error occurred while processing your request.");
//            }
//        }

//        [HttpDelete("delete-courses/{courseId}")]
//        public async Task<ActionResult> DeleteCourse([FromHeader(Name = "Authorization")] string token, int courseId)
//        {
//            try
//            {
//                string instructorId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

//                if (string.IsNullOrEmpty(instructorId))
//                {
//                    return BadRequest("Invalid token or token expired.");
//                }

//                // Retrieve the course to delete
//                var courseToDelete = await _dbcontext.Courses.FindAsync(courseId);

//                if (courseToDelete == null)
//                {
//                    return NotFound("Course not found.");
//                }

//                // Ensure that the course belongs to the instructor
//                if (courseToDelete.InstructorID != instructorId)
//                {
//                    return Unauthorized("You are not authorized to delete this course.");
//                }

//                // Remove the course from the database
//                _dbcontext.Courses.Remove(courseToDelete);
//                await _dbcontext.SaveChangesAsync();

//                return Ok();
//            }
//            catch (Exception ex)
//            {
//                // Log the exception
//                //_logger.LogError(ex, "An error occurred while deleting the course.");
//                return StatusCode(500, "An error occurred while processing your request.");
//            }
//        }













//        #region private method
//        private async Task<int> GetCategoryIdByName(string categoryName)
//        {
//            var category = await _dbcontext.Categories.FirstOrDefaultAsync(c => c.Name == categoryName);
//            return category?.Id ?? 0;
//        }

//        private async Task<string> SaveFileAsync(IFormFile file, string folderPath)
//        {
//            if (file != null && file.Length > 0)
//            {
//                var uploadsFolder = Path.Combine(_hostEnvironment.WebRootPath, folderPath);
//                if (!Directory.Exists(uploadsFolder))
//                {
//                    Directory.CreateDirectory(uploadsFolder);
//                }

//                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
//                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

//                using (var stream = new FileStream(filePath, FileMode.Create))
//                {
//                    await file.CopyToAsync(stream);
//                }

//                return Path.Combine(folderPath, uniqueFileName);
//            }

//            return null;
//        }

//        private void UpdateCourse(Course course, InstructorDashboardDataToPostDto courseData, IFormFile coverImage)
//        {
//            course.Price = courseData.Price;
//            course.Level = courseData.Level;
//            course.Language = courseData.Language;
//            course.BriefDescription = courseData.CourseDisc;
//            course.DateUpdated = DateTime.UtcNow;
//            course.FullDescription = courseData.FullDescription;
//            course.CategoryID = GetCategoryIdByName(courseData.CategoryName).Result; // Ensure it's synchronous
//            course.Cover = SaveFileAsync(coverImage, "courses/coursesCovers").Result; // Ensure it's synchronous
//        }
//        private Course CreateCourse(InstructorDashboardDataToPostDto courseData, string instructorId, IFormFile coverImage)
//        {
//            return new Course
//            {
//                Approved = true,
//                DateCreated = DateTime.UtcNow,
//                Name = courseData.CourseTitle,
//                Price = courseData.Price,
//                Level = courseData.Level,
//                Language = courseData.Language,
//                BriefDescription = courseData.CourseDisc,
//                DateUpdated = DateTime.UtcNow,
//                FullDescription = courseData.FullDescription,
//                InstructorID = instructorId,
//                CategoryID = GetCategoryIdByName(courseData.CategoryName).Result, // Ensure it's synchronous
//                Cover = SaveFileAsync(coverImage, "courses/coursesCovers").Result // Ensure it's synchronous
//            };
//        }
//        private async Task CreateOrUpdateCourseRequirements(Course course, IEnumerable<RequirementDto> requirements)
//        {
//            if (requirements != null)
//            {
//                _dbcontext.Requirements.RemoveRange(course.Requirements); // Remove existing requirements
//                await _dbcontext.SaveChangesAsync();

//                foreach (var requirementDto in requirements)
//                {
//                    var requirement = new Requirement
//                    {
//                        Description = requirementDto.Content,
//                        CourseID = course.CourseID
//                    };
//                    _dbcontext.Requirements.Add(requirement);
//                }
//            }
//        }

//        private async Task CreateOrUpdateCourseObjectives(Course course, IEnumerable<ObjectiveDto> objectives)
//        {
//            if (objectives != null)
//            {
//                _dbcontext.Objectives.RemoveRange(course.Objectives); // Remove existing objectives
//                await _dbcontext.SaveChangesAsync();

//                foreach (var objectiveDto in objectives)
//                {
//                    var objective = new Objective
//                    {
//                        Description = objectiveDto.Content,
//                        CourseID = course.CourseID
//                    };
//                    _dbcontext.Objectives.Add(objective);
//                }
//            }
//        }
//        private async Task CreateOrUpdateCourseSections(Course course,SectionWithoutFile section, List<IFormFile> lessonFiles)
//        {
//            if (section != null)
//            {
//                _dbcontext.Sections.RemoveRange(course.Sections); // Remove existing sections
//                await _dbcontext.SaveChangesAsync();

             
//                    var newSection = new Section
//                    {
//                        Title = section.SectionName,
//                        CourseID = course.CourseID,
                       

//                    };


//                    _dbcontext.Sections.Add(newSection);

//                CreateOrUpdateSectionLessons(newSection, section.Lessons, course.CourseID, lessonFiles);






//            }
//        }
//        private async Task CreateOrUpdateSectionLessons(Section section, IEnumerable<LessonWithoutFileDto> lessons, int courseId, List<IFormFile> lessonFiles)
//        {
//            if (lessons != null)
//            {
//                for (int i = 0; i < lessons.Count(); i++)
//                {
//                    var lessonDto = lessons.ElementAt(i);
//                    var lessonFile = lessonFiles[i]; // Get corresponding lesson file
//                    var lessonFilePath = await SaveFileAsync(lessonFile, $"courses/{courseId}/{section.Title}");
//                    var lesson = new Lesson
//                    {
//                        Title = lessonDto.LessonName,
//                        SectionID = section.SectionID,
//                        File = lessonFilePath, // Assign lesson file path
//                        Duration = lessonDto.LessonTimeInMinutes,
//                    };
//                    _dbcontext.Lessons.Add(lesson);
//                }
//                await _dbcontext.SaveChangesAsync();
//            }
//        }

//        #endregion
//    }
//}
