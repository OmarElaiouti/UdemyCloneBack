using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAl.Models;
using Udemy.DAL.DTOs;


namespace Udemy.DAL.Interfaces
{
    public interface IUserRepository
    {
        Task<User> GetUserByIdAsync(string userId);
        Task<UserDto> GetUserDtoByIdAsync(string userId);
        Task<UserDto> UpdateUserAsync(string userId, UserDto userDto);
        Task<Transaction> CreateAndProcessTransactionAsync(string userId);
        Task<string> UpdateUserImage(string userId, string filePath);
        Task DeleteUserAsync(string userId);
        Task<IEnumerable<NotificationDto>> GetUserNotifications(string userId);
        Task UpdateLastFiveNotificationsStatus(string userId);
        Task SetAllUserNotificationsStatus(string userId);

        Task<bool> AddInstructorRoleToUser(string userId);
    }
}
