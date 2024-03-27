using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;

namespace Udemy.Core.Services
{
    public class CourseService:ICourseService
    {
        private readonly ICourseRepository<Course> _courseRepository;

        public CourseService(ICourseRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public List<SearchCourseDto> SearchCourses(string searchString)
        {
            // Define the predicate
            Expression<Func<Course, bool>> predicate = c => c.Name.Contains(searchString) || c.BriefDescription.Contains(searchString);

            // Define includes for related entities and nested properties
            Expression<Func<Course, object>>[] includes =
            {
            c => c.Instructor,

        };

            // Call SearchAsync method with predicate and includes
            var courses = _courseRepository.Search(predicate, includes);

            // Map the result to DTOs
            return courses.Select(MapToDto).ToList();
        }

        private SearchCourseDto MapToDto(Course course)
        {
            return new SearchCourseDto
            {
                Name = course.Name,
                BriefDescription = course.BriefDescription,
                InstructorName = course.Instructor?.Name ?? "hala",
                //Rate = CalculateAverageRate(course),
                //Price = course.Price,
                //TotalLessons = course.Sections?.Sum(section => section.Lessons.Count) ?? 0,
                //TotalHours = course.Sections?.Sum(section => section.Lessons.Sum(lesson => lesson.Duration)) ?? 0
            };
        }

        private float CalculateAverageRate(Course course)
        {
            if (course.Enrollments == null || course.Enrollments.Count == 0)
                return 0;

            return course.Enrollments.Average(enrollment => enrollment.Feedback?.Rate ?? 0);
        }
    }
}
