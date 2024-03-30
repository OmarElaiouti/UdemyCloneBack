using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public UserController(IUserRepository usereService, IAuthService authService)
        {
            _userService = usereService;
            _authService = authService;

        }

        [HttpGet("get-user")]
        public async Task<ActionResult<List<UserDto>>> GetCoursesInCartByUserId([FromHeader(Name = "token")] string token)
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
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

        [HttpPut("edit-user-data")]
        public async Task<IActionResult> UpdateUser([FromHeader(Name = "token")] string token, [FromBody] UserDto userDto)
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
        public async Task<IActionResult> CreateEnrollment([FromHeader(Name = "token")] string token)
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

                return Ok();
            }
            catch (Exception ex)
            {
                // Handle exception (e.g., return error response)
                return StatusCode(500, "Failed to create enrollment: " + ex.Message);
            }
        }

        [HttpDelete("delete-account")]
        public async Task<IActionResult> DeleteAccount([FromHeader(Name = "token")] string token)
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
    }
}
