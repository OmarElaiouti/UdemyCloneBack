using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System.Security.Claims;
using Udemy.BLL.Interfaces;
using Udemy.DAL.DTOs;


namespace UdemyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [Authorize]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;


        public UserController(ILogger<UserController> logger, IUserService usereService, IWebHostEnvironment hostEnvironment)
        {
            _userService = usereService;
            _logger = logger;
        }

        [HttpGet("get-user")]
        public async Task<ActionResult<UserDto>> GetUser()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var user = await _userService.GetUserDtoByIdAsync(userId);

                if (user == null)
                {
                    return NotFound("courses not found in the user cart.");
                }
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(500);
            }
        }


        [HttpPost("edit-user-data")]
        public async Task<IActionResult> UpdateUser([FromBody] UserDto userDto)
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }
                var updatedUser = await _userService.UpdateUserAsync(userId, userDto);
                return Ok(updatedUser); // Return the updated user
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, $"An error occurred");
            }
        }

        [HttpPost("create-transaction")]
        public async Task<IActionResult> CreateEnrollment()
        {
            try
            {
                // Check if transactionDto is valid and contains necessary data
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                // Create transaction and process it
                var transaction = await _userService.CreateAndProcessTransactionAsync(userId);

                return Ok(new { transaction.Status ,transaction.Date});
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Failed to create enrollment");
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount()
        {
            try
            {
                // Retrieve the user ID from the claims in the authentication token
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                // Delete the user account using the repository
                await _userService.DeleteUserAsync(userId);

                return Ok("Account deleted successfully.");
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError("Error deleting account: " + ex.Message);
                return StatusCode(500, "An error occurred while deleting the account.");
            }
        }



        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromBody] string filePath)
        {
            try
            {
                // Check if user exists
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var (success, message) = await _userService.UpdateUserImage(userId, filePath);

                if (success)
                {
                    return Ok(new { message = "User image updated successfully.", filePath = message });
                }
                else
                {
                    return BadRequest(new { error = message });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating user image: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error updating user image");
            }
        }

        [HttpGet("add-instructor-role")]
        public async Task<IActionResult> AddRoleToUser()
        {
            try
            {
                // Validate token
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest();
                }

                // Add role to user
                var result = await _userService.AddInstructorRoleToUser(userId);
                if (result)
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError("Error occurred while adding role to user.",ex.Message);

                return StatusCode(500, "An error occurred while processing your request.");
            }
        }


    }
}
