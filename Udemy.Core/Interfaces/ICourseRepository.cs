using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<SearchCourseDto>> SearchCoursesByNameAsync(string searchString);
        Task<IEnumerable<CourseCardWithRateDto>> SearchCoursesByNameAsync(string searchString, int count, bool smallCard);
        Task<IEnumerable<CourseCardWithRateDto>> GetRandomCourses(int count);
        Task<IEnumerable<CourseCardWithRateDto>> GetTopRatedCourses(int count);
        Task<IEnumerable<CourseCardWithRateDto>> GetCoursesByCategory(string categoryName, int? count);

        Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesInCartByUserId(string id);
        Task<IEnumerable<CourseCardWithRateDto>> GetCoursesInWishlistByUserId(string id);
        Task<IEnumerable<CourseCardWithRateDto>> GetEnrolledInCoursesByUserId(string id);

        Task<Course> GetCourseById(int id);
        Task AddCourseToCartByUserIdAsync(string userId, int courseId);
        Task AddCourseToWishlistByUserIdAsync(string userId, int courseId);


    }
}
