using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Udemy.BLL.Interfaces;
using Udemy.DAL.DTOs;

namespace Udemy.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class Notifications : ControllerBase
    {
        private readonly IUserService _userService;

        public Notifications(IUserService usereService)
        {
            _userService = usereService;
        }

       


        [HttpGet("user-notifications")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetUserNotifications()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }
                var notifications = await _userService.GetUserNotifications(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {

                return StatusCode(500, $"An error occurred while retrieving user notifications.");
            }
        }

        [HttpPost("notifications-status-last-five")]
        [Authorize]
        public async Task<ActionResult> SetLastUserNotificationsStatus()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }


                await _userService.UpdateLastFiveNotificationsStatusToTrue(userId);
                return Ok("Last five notifications status updated successfully.");
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"An error occurred while updating last five notifications' status: {ex.Message}");
            }
        }

        [HttpGet("notifications-status")]
        [Authorize]
        public async Task<ActionResult> SetAllUserNotificationsStatus()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                await _userService.SetAllUserNotificationsStatusToTrue(userId);
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
