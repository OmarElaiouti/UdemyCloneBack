using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.Repository;

namespace Udemy.Core.Services
{
    
    public class CourseRepository:BaseRepository<Course>, ICourseRepository
    {
        private readonly IBaseRepository<Course> _courseRepository;
        private readonly ICartRepository _cartRepository;


        public CourseRepository(IBaseRepository<Course> courseRepository, ICartRepository cartRepository, UdemyContext dbContext):base(dbContext)
        {
            _courseRepository = courseRepository;
            _cartRepository = cartRepository;
        }

        public async Task<IEnumerable<SearchCourseDto>> SearchCoursesByNameAsync(string searchString)
        {
            var allCourses = await GetAllWithIncluded();
            var matchingCourses = allCourses.Where(c => c.Name.Contains(searchString));


                return MapToSearchCourseDto(matchingCourses).ToList();
            
           
        }
        public async Task<IEnumerable<CourseCardWithRateDto>> SearchCoursesByNameAsync(string searchString, int count,bool smallCard)
        {
            var allCourses = await GetAllWithIncluded();
            var matchingCourses = allCourses.Where(c => c.Name.Contains(searchString));
            if (smallCard)
            {
                matchingCourses = matchingCourses.Take(count);
            }

            return MapToCourseCardWithRateDto(matchingCourses).ToList();
        }


        public async Task<IEnumerable<CourseCardWithRateDto>> GetRandomCourses(int count)
        {
            var courses = await GetAllWithIncluded();
            var randomCourses = courses.OrderBy(c => Guid.NewGuid()).Take(count);
            return MapToCourseCardWithRateDto(randomCourses).ToList();
        }    
        public async Task<IEnumerable<CourseCardWithRateDto>> GetTopRatedCourses(int count)
        {
            var courses = await GetAllWithIncluded();
            var topRatedCourses = courses.OrderByDescending(c => c.Enrollments.Average(e => e.Feedback.Rate)).Take(count);
            return MapToCourseCardWithRateDto(topRatedCourses).ToList();
        }
        public async Task<IEnumerable<CourseCardWithRateDto>> GetCoursesByCategory(string categoryName, int? count)
        {
            var courses = await GetAllWithIncluded();

            var filteredCourses = courses
                .Where(course => course.Categories.Any(category => category.Name == categoryName));

            if (count.HasValue)
            {
                filteredCourses = filteredCourses.Take(count.Value);
            }

            return MapToCourseCardWithRateDto(filteredCourses).ToList();

        }



        public async Task<IEnumerable<CourseCardWithRateDto>> GetCoursesInCartByUserId(string id)
        {
            var cart = await _cartRepository.GetCartByUserIdAsync(id);

            return MapToCourseCardWithRateDto(cart.CoursesInCart).ToList();

        }



        #region private methods
        private async Task<IEnumerable<Course>> GetAllWithIncluded()
        {
                    var courses = await _courseRepository.GetAll(
                includeRelatedEntities: true,
                c => c.Instructor,
                c => c.Enrollments,
                c => c.Enrollments.Select(e => e.Feedback),
                c => c.Sections.Select(s => s.Lessons)
            );

            // Ensure the courses are materialized as a list
            return courses;
        }
        private IEnumerable<CourseCardWithRateDto> MapToCourseCardWithRateDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new CourseCardWithRateDto
            {
                ID = course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                InstructorName = course.Instructor?.Name ?? "Unknown",
                Rate = CalculateAverageRate(course),
                Price = course.Price
            });
        }
        private IEnumerable<SearchCourseDto> MapToSearchCourseDto(IEnumerable<Course> courses)
        {
            return courses.Select(course => new SearchCourseDto
            {
                ID=course.CourseID,
                Image = course.Cover,
                Name = course.Name,
                BriefDescription = course.BriefDescription,
                InstructorName = course.Instructor?.Name ?? "Unknown",
                Rate = CalculateAverageRate(course),
                Price = course.Price,
                TotalLessons = course.Sections?.Sum(section => section.Lessons.Count) ?? 0,
                TotalHours = course.Sections?.Sum(section => section.Lessons.Sum(lesson => lesson.Duration)) ?? 0
            });
        }
        
        private float CalculateAverageRate(Course course)
        {
            if (course.Enrollments == null || course.Enrollments.Count == 0)
                return 0;

            return course.Enrollments.Average(enrollment => enrollment.Feedback?.Rate ?? 0);
        
        }

    #endregion

    }
}
