using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Udemy.BLL.Services.Interfaces;
using Udemy.CU.Exceptions;
using Udemy.DAl.Models;
using Udemy.DAL.GenericBaseRepository.BaseRepository;
using Udemy.DAL.Context;
using Udemy.DAL.DTOs;
using Udemy.DAL.DTOs.CourseDtos;
using Udemy.DAL.DTOs.CoursePartsDtos;
using Udemy.DAL.StaticClasses;

namespace Udemy.BLL.Services
{
    public class InstructorService : IInstructorService
    {
        private readonly UserManager<User> _userManager;
        private readonly UdemyContext _context;
        private readonly IBaseRepository<User> _userRepository;



        public InstructorService(IBaseRepository<User> userRepository, UserManager<User> userManager, UdemyContext udemyContext)
        {
            _userManager = userManager;
            _context = udemyContext;
            _userRepository = userRepository;
        }

        public async Task<IEnumerable<InstructorDto>> GetAllInstructors()
        {
            var instructors = await _userRepository.GetAllAsync();
            return instructors.Select(instructor => new InstructorDto
            {
                InstructorId = instructor.Id,
                Name = $"{instructor.FirstName ?? "Unknown"} {instructor.LastName ?? "Unknown"}",
                Image = instructor.Image ?? "",
                HeadLine = instructor.Headline ?? "",
                Biography = instructor.Biography ?? "",
                Rate = instructor.CreatedCourses
                                .SelectMany(cc => cc.Enrollments)
                                .Where(e => e.Feedback != null)
                                .Select(e => e.Feedback.Rate)
                                .Average(),
                CoursesCount = instructor.CreatedCourses.Count,
                StudentsCount = instructor.CreatedCourses.SelectMany(cc => cc.Enrollments)
                                        .Select(e => e.UserId)
                                        .Distinct()
                                        .Count(),
                FeedbacksCount = instructor.CreatedCourses
                                        .SelectMany(cc => cc.Enrollments)
                                        .Count(e => e.Feedback != null),
            });

        }       
        public async Task<InstructorWithHisCoursesDto> GetInstructorById(string instructorId)
        {
            var instructor = await _userManager.FindByIdAsync(instructorId);
            if (instructor == null)
            {
                throw new NotFoundException($"Instructor with ID '{instructorId}' not found.");
            }

            return new InstructorWithHisCoursesDto
            {
                InstructorId = instructor.Id,
                Name = $"{instructor.FirstName ?? "Unknown"} {instructor.LastName ?? "Unknown"}",
                Image = instructor.Image ?? "",
                HeadLine = instructor.Headline ?? "",
                Biography = instructor.Biography ?? "",
                Rate = instructor.CreatedCourses
                                .SelectMany(cc => cc.Enrollments)
                                .Where(e => e.Feedback != null)
                                .Select(e => e.Feedback.Rate)
                                .Average(),
                CoursesCount = instructor.CreatedCourses.Count,
                Courses = instructor.CreatedCourses.Select(c => new CourseWithObjectivesDto
                {
                    ID = c.CourseID,
                    Image = c.Cover ?? "",
                    Name = c.Name,
                    InstructorName = instructor.FirstName ?? "Unknown" + " " + instructor.FirstName ?? "Unknown",
                    Rate = Calculations.CalculateAverageRate(c),
                    Price = c.Price,
                    ReviewersNumber = c.Enrollments.Count(e => e.Feedback != null),
                    Objectives = c.Objectives.Select(o => new ObjectiveDto
                    {
                        ID = o.ObjectiveID,
                        Content = o.Description
                    }

                    )


                }) ?? Enumerable.Empty<CourseWithObjectivesDto>(),
                StudentsCount = instructor.CreatedCourses.SelectMany(cc => cc.Enrollments)
                                        .Select(e => e.UserId)
                                        .Distinct()
                                        .Count(),
                FeedbacksCount = instructor.CreatedCourses
                                        .SelectMany(cc => cc.Enrollments)
                                        .Count(e => e.Feedback != null),
            };       

        }

        public async Task<IEnumerable<InstructorDto>> GetInstructorsByCategoryName(string categoryName)
        {
            var instructors = await _context.Users
            .Where(u => u.CreatedCourses.Any(c =>
                                            c.Category.Name == categoryName ||
                                            c.Category.ParentCategory.Name == categoryName ||
                                            c.Category.ParentCategory.ParentCategory.Name == categoryName))
                                            .ToListAsync();

            if (!instructors.Any())
            {
                throw new NotFoundException($"Instructors with courses in '{categoryName}' Category");
            }

            return instructors.Select(instructor => new InstructorDto
            {
                InstructorId = instructor.Id,
                Name = $"{instructor.FirstName ?? "Unknown"} {instructor.LastName ?? "Unknown"}",
                Image = instructor.Image ?? "",
                HeadLine = instructor.Headline ?? "",
                Biography = instructor.Biography ?? "",
                Rate = instructor.CreatedCourses
                                .SelectMany(cc => cc.Enrollments)
                                .Where(e => e.Feedback != null)
                                .Select(e => e.Feedback.Rate)
                                .Average(),
                CoursesCount = instructor.CreatedCourses.Count,
                StudentsCount = instructor.CreatedCourses.SelectMany(cc => cc.Enrollments)
                                        .Select(e => e.UserId)
                                        .Distinct()
                                        .Count(),
                FeedbacksCount = instructor.CreatedCourses
                                        .SelectMany(cc => cc.Enrollments)
                                        .Count(e => e.Feedback != null),
            });
        }
    }
}
