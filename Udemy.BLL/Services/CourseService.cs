using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Udemy.BLL.Interfaces;
using Udemy.DAl.Models;
using Udemy.DAL.GenericBaseRepository.BaseRepository;
using Udemy.DAL.Context;
using Udemy.DAL.DTOs.CourseDtos;
using Udemy.DAL.StaticClasses;
using Udemy.DAL.UnitOfWork;


namespace Udemy.BLL.Services
{

    public class CourseService:ICourseService
    {
        private readonly IBaseRepository<Course> _courseRepository;

        public CourseService(IBaseRepository<Course> courseRepository,
            UdemyContext dbContext)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CourseLongDto>> SearchCoursesByNameAsync(string searchString)
        {
            var matchingCourses = await _courseRepository.GetAllAsync(c => c.Name.Contains(searchString) || c.BriefDescription.Contains(searchString));

            return Mappers.MapToLongCourseDto(matchingCourses).ToList();
                       
        }
        public async Task<IEnumerable<CourseWithObjectivesDto>> SearchCoursesByNameWithObjectivesAsync(string searchString, int count)
        {
            var matchingCourses = await _courseRepository.GetAllAsync(c => c.Name.Contains(searchString) || c.BriefDescription.Contains(searchString));
            matchingCourses = matchingCourses.Take(count);
           

            return Mappers.MapToCourseWithObjectivesDtoDto(matchingCourses).ToList();
        }
        public async Task<IEnumerable<CourseWithObjectivesDto>> GetRandomCourses(int count)
        {
            var allCourses = await _courseRepository.GetAllAsync();
            var random = new Random();
            var randomCourses = allCourses.OrderBy(c => random.Next()).Take(count); 
            
            return Mappers.MapToCourseWithObjectivesDtoDto(randomCourses).ToList();
        }    
        public async Task<IEnumerable<CourseWithObjectivesDto>> GetTopRatedCourses(int count)
        {
            var courses = await _courseRepository.GetAllAsync();
            var topRatedCourses = courses.OrderByDescending(c => c.Enrollments.Average(e => e.Feedback.Rate)).Take(count);
            return Mappers.MapToCourseWithObjectivesDtoDto(topRatedCourses).ToList();
        }
        public async Task<IEnumerable<CourseLongDto>> GetCoursesByCategory(string categoryName, int? count)
        {
            var filteredCourses = await _courseRepository.GetAllAsync(course => course.Category.Name == categoryName);

            if (count.HasValue)
            {
                filteredCourses = filteredCourses.Take(count.Value);
            }

            return Mappers.MapToLongCourseDto(filteredCourses).ToList();

        }
        public async Task<IEnumerable<CourseWithObjectivesDto>> GetCoursesByCategoryWithObjctives(string categoryName, int? count)
        {
            var filteredCourses = await _courseRepository.GetAllAsync(course => course.Category.Name == categoryName);

            if (count.HasValue)
            {
                filteredCourses = filteredCourses.Take(count.Value);
            }

            return Mappers.MapToCourseWithObjectivesDtoDto(filteredCourses).ToList();

        }
        public async Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesByIds(List<int> itemIds)
        {
            try
            {
                var courses = new List<Course>();

                foreach (var itemId in itemIds)
                {
                    var course = await _courseRepository.GetByIdAsync(itemId);
                    if (course != null)
                    {
                        courses.Add(course);
                    }
                }

                return Mappers.MapToCourseCardWithLevelDto(courses).ToList();
            }
            catch (Exception ex)
            {
                // Handle exception appropriately, e.g., log the error
                Console.WriteLine($"An error occurred while fetching courses: {ex.Message}");
                throw; // Rethrow the exception for further handling
            }
        }

       



    }
}
