using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using Udemy.EF.Repository;
using UdemyCloneBackend.Services;

namespace UdemyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseService;
        private readonly ICartRepository _cartService;

        private readonly IAuthService  _authService;

        public CoursesController(ICourseRepository courseService, ICartRepository cartService, IAuthService authService)
        {
            _courseService = courseService;
            _authService = authService;
            _cartService = cartService;
        }

        [HttpGet("searched-courses")]
        public async Task<ActionResult<List<SearchCourseDto>>> GetSearchCoursesByNameAsync([FromQuery] string searchString)
        {
            try
            {
                var courses = await _courseService.SearchCoursesByNameAsync(searchString);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

        [HttpGet("saved-search")]
        public async Task<ActionResult<List<CourseCardWithRateDto>>> GetSavedSearchCoursesByNameAsync([FromQuery] string searchHitory)
        {
            try
            {
                var courses = await _courseService.SearchCoursesByNameAsync(searchHitory,5,true);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("courses-in-cart")]
        public async Task<ActionResult<List<CourseCardWithLevelDto>>> GetCoursesInCartByUserId([FromHeader(Name = "token")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var courses = await _courseService.GetCoursesInCartByUserId(userId);

                if (courses == null)
                {
                    return NotFound("courses not found in the user cart.");
                }
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }



        [HttpGet("courses-in-wishlist")]
        public async Task<ActionResult<List<CourseCardWithRateDto>>> GetCoursesInWishlistlistById([FromHeader(Name = "token")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var courses = await _courseService.GetCoursesInWishlistByUserId(userId);

                if (courses == null)
                {
                    return NotFound("courses not found in the user cart.");
                }
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpGet("enrolled-in")]
        public async Task<ActionResult<List<CourseCardWithRateDto>>> GetEnrolledInCoursesById([FromHeader(Name = "token")] string token)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var courses = await _courseService.GetEnrolledInCoursesByUserId(userId);

                if (courses == null)
                {
                    return NotFound("courses not found in the user cart.");
                }
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }




        [HttpGet("random")]
        public async Task<IActionResult> GetRandomCourses()
        {
            try
            {
                var randomCourses = await _courseService.GetRandomCourses(4);
                return Ok(randomCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-rated")]
        public async Task<IActionResult> GetTopRatedCourses()
        {
            try
            {
                var topRatedCourses = await _courseService.GetTopRatedCourses(4); // Get top 10 rated courses
                return Ok(topRatedCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-category")]
        public async Task<IActionResult> GetCoursesByCategory(string categoryName, [FromQuery] int? num)
        {
            try
            {
                var courses = await _courseService.GetCoursesByCategory(categoryName, num);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }




        [HttpPost]
        [Route("addCourseToCart")]
        public async Task<IActionResult> AddCourseToCart([FromHeader(Name = "token")] string token,[FromBody] int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                await _courseService.AddCourseToCartByUserIdAsync(userId, courseId);
                return Ok("Course added to cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost]
        [Route("addCourseToWishlist")]
        public async Task<IActionResult> AddCourseToWishlist([FromHeader(Name = "token")] string token, [FromBody] int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                await _courseService.AddCourseToWishlistByUserIdAsync(userId, courseId);
                return Ok("Course added to cart successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

    }









}
