using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.EF.Repository;
using UdemyCloneBackend.Services;

namespace UdemyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _userService;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _hostEnvironment;

        public UserController(IUserRepository usereService, IAuthService authService, IWebHostEnvironment hostEnvironment)
        {
            _userService = usereService;
            _authService = authService;
            _hostEnvironment = hostEnvironment;
        }

        [HttpGet("get-user")]
        public async Task<ActionResult<UserDto>> GetUser([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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
                return StatusCode(500, ex);
            }
        }

        [HttpPut("edit-user-data")]
        public async Task<IActionResult> UpdateUser([FromHeader(Name = "Authorization")] string token, [FromBody] UserDto userDto)
        {
            try
            {

                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("create-transaction")]
        public async Task<IActionResult> CreateEnrollment([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                // Check if transactionDto is valid and contains necessary data
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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
                // Handle exception (e.g., return error response)
                return StatusCode(500, "Failed to create enrollment: " + ex);
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "Authorization")] string token)
        {
            try
            {
                // Retrieve the user ID from the claims in the authentication token
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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
                Console.WriteLine("Error deleting account: " + ex.Message);
                return StatusCode(500, "An error occurred while deleting the account.");
            }
        }



        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage([FromHeader(Name = "Authorization")] string token, IFormFile file)
        {
            try
            {
                // Check if user exists
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                // Check if the file exists
                if (file == null || file.Length == 0)
                {
                    return BadRequest("Invalid file.");
                }

                // Define the upload directory
                var uploadDir = Path.Combine(_hostEnvironment.WebRootPath, "userImages");

                // Create the directory if it doesn't exist
                if (!Directory.Exists(uploadDir))
                {
                    Directory.CreateDirectory(uploadDir);
                }

                // Define the file path
                var filePath = Path.Combine(uploadDir, $"{userId}.jpg");

                // Save the file to the server
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                await _userService.UpdateUserImage(userId,filePath);

                return Ok("Image uploaded successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading image: {ex.Message}");
            }
        }

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

    }
}
