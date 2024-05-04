using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAl.Models;
using Udemy.DAL.DTOs;
using Udemy.DAL.DTOs.CourseDtos;


namespace Udemy.BLL.Interfaces
{
    public interface IUserService
    {


        Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesInCartByUserId(string id);
        Task<IEnumerable<CourseLongDto>> GetCoursesInWishlistByUserId(string id);
        Task<IEnumerable<CourseLongDto>> GetEnrolledInCoursesByUserId(string id);
        Task<CourseShortDto> AddCourseToCartByUserIdAsync(string userId, int courseId);
        Task<CourseShortDto> AddCourseToWishlistByUserIdAsync(string userId, int courseId);
        Task<bool> RemoveCourseFromCartByUserIdAsync(string userId, int courseId);
        Task<bool> RemoveCourseFromWishlistByUserIdAsync(string userId, int courseId);

        Task<UserDto> GetUserDtoByIdAsync(string userId);
        Task<UserDto> UpdateUserAsync(string userId, UserDto userDto);
        Task<Transaction> CreateAndProcessTransactionAsync(string userId);
        Task<(bool success, string message)> UpdateUserImage(string userId, string filePath);        
        Task DeleteUserAsync(string userId);
        Task<IEnumerable<NotificationDto>> GetUserNotifications(string userId);
        Task UpdateLastFiveNotificationsStatusToTrue(string userId);
        Task SetAllUserNotificationsStatusToTrue(string userId);

        Task<bool> AddInstructorRoleToUser(string userId);
    }
}
