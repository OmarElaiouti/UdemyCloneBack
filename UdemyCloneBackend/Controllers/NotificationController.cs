using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using UdemyCloneBackend.Services;

namespace Udemy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Notifications : ControllerBase
    {
        private readonly IUserRepository _userService;
        private readonly IAuthService _authService;
        private readonly IBaseRepository<Notification> _repository;

        public Notifications(IBaseRepository<Notification> repository, IUserRepository usereService, IAuthService authService)
        {
            _repository = repository;
            _userService = usereService;
            _authService = authService;
        }

        //[HttpGet("Notification")]
        //public ActionResult<IEnumerable<Notification>> Notififcation()
        //{
        //    return _repository.GetAll2().ToList();
        //}




        [HttpGet("user-notifications")]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }
                var notifications = await _userService.GetUserNotifications(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving user notifications.");
            }
        }

        [HttpPost("notifications-status-last-five")]
        public async Task<ActionResult> SetLastUserNotificationsStatus([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }


                await _userService.UpdateLastFiveNotificationsStatus(userId);
                return Ok("Last five notifications status updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"An error occurred while updating last five notifications' status: {ex.Message}");
            }
        }

        [HttpGet("notifications-status")]
        public async Task<ActionResult> SetAllUserNotificationsStatus([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                await _userService.SetAllUserNotificationsStatus(userId);
                return Ok("All notifications status updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"An error occurred while updating all notifications' status: {ex.Message}");
            }
        }
    }
}
