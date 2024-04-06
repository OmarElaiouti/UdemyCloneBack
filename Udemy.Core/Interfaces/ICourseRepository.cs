using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface ICourseRepository
    {
        Task<IEnumerable<CourseLongDto>> SearchCoursesByNameAsync(string searchString);
        Task<IEnumerable<CourseWithObjectivesDto>> SearchCoursesByNameAsync(string searchString, int count);
        Task<IEnumerable<CourseWithObjectivesDto>> GetRandomCourses(int count);
        Task<IEnumerable<CourseWithObjectivesDto>> GetTopRatedCourses(int count);
        Task<IEnumerable<CourseLongDto>> GetCoursesByCategory(string categoryName, int? count);

        Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesInCartByUserId(string id);
        Task<IEnumerable<CourseLongDto>> GetCoursesInWishlistByUserId(string id);
        Task<IEnumerable<CourseLongDto>> GetEnrolledInCoursesByUserId(string id);

        Task<Course> GetCourseById(int id);
        Task<CourseShortDto> AddCourseToCartByUserIdAsync(string userId, int courseId);
        Task<CourseShortDto> AddCourseToWishlistByUserIdAsync(string userId, int courseId);
        Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesByIds(List<int> itemIds);
        Task<IEnumerable<CourseWithObjectivesDto>> GetCoursesByCategoryWithObjctives(string categoryName, int? count);
        Task<bool> RemoveCourseFromCartByUserIdAsync(string userId, int courseId);
        Task<bool> RemoveCourseFromWishlistByUserIdAsync(string userId, int courseId);

    }
}
