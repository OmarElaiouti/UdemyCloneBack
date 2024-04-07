using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs.CourseDtos;
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
        public async Task<ActionResult<List<CourseLongDto>>> GetSearchCoursesByNameAsync([FromQuery] string searchString)
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
        public async Task<ActionResult<List<CourseWithObjectivesDto>>> GetSavedSearchCoursesByNameAsync([FromQuery] string searchHitory)
        {
            try
            {
                var courses = await _courseService.SearchCoursesByNameAsync(searchHitory,5);
                return Ok(courses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }

        [HttpPost("anonymous-cart")]
        public async Task<ActionResult<List<CourseCardWithLevelDto>>> GetAnonymousCart()
        {
            try
            {
                var itemIds = Request.Form["itemIds"].Select(int.Parse).ToList();

                Console.WriteLine("Received itemIds: " + string.Join(",", itemIds));

                // Assuming you have a repository/service to handle cart operations
                var cartItems = await _courseService.GetCoursesByIds(itemIds);

                // Return the cart items as JSON response
                return Ok(cartItems);
            }
            catch (Exception ex)
            {
                // Handle errors
                return StatusCode(500, new { message = "Internal server error" });
            }
        }

        [HttpGet("courses-in-cart")]
        public async Task<ActionResult<List<CourseCardWithLevelDto>>> GetCoursesInCartByUserId([FromHeader(Name = "Authorization")] string token)
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
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

        
    

    [HttpGet("courses-in-wishlist")]
        public async Task<ActionResult<List<CourseLongDto>>> GetCoursesInWishlistlistById([FromHeader(Name = "Authorization")] string token)
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
        public async Task<ActionResult<List<CourseLongDto>>> GetEnrolledInCoursesById([FromHeader(Name = "Authorization")] string token)
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
        public async Task<ActionResult<List<CourseWithObjectivesDto>>> GetRandomCourses()
        {
            try
            {
                var randomCourses = await _courseService.GetRandomCourses(6);
                return Ok(randomCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("top-rated")]
        public async Task<ActionResult<List<CourseWithObjectivesDto>>> GetTopRatedCourses()
        {
            try
            {
                var topRatedCourses = await _courseService.GetTopRatedCourses(6); // Get top 10 rated courses
                return Ok(topRatedCourses);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("by-category")]
        public async Task<ActionResult<List<CourseLongDto>>> GetCoursesByCategory([FromQuery] string categoryName, [FromQuery] int? num)
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

        [HttpGet("by-category-with-objectives")]
        public async Task<ActionResult<List<CourseWithObjectivesDto>>> GetCoursesWithObjectivesByCategory([FromQuery] string categoryName, [FromQuery] int? num)
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



        [HttpGet("add-course-to-cart/{courseId}")]
        public async Task<IActionResult> AddCourseToCart([FromHeader(Name = "Authorization")] string token,int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var addedCourse = await _courseService.AddCourseToCartByUserIdAsync(userId, courseId);
                return Ok(addedCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

        [HttpGet("addCourseToWishlist/{courseId}")]
        public async Task<IActionResult> AddCourseToWishlist([FromHeader(Name = "Authorization")] string token, int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var addedCourse = await _courseService.AddCourseToWishlistByUserIdAsync(userId, courseId);
                return Ok(addedCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        }

        [HttpGet("RemoveCourseFromWishlist/{courseId}")]
        public async Task<IActionResult> RemoveCourseFromWishlist([FromHeader(Name = "Authorization")] string token, int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var RemovedCourse = await _courseService.RemoveCourseFromWishlistByUserIdAsync(userId, courseId);
                return Ok("The course removed from the whishlist successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        }

        [HttpGet("remove-course-from-cart/{courseId}")]
        public async Task<IActionResult> RemoveCourseFromCart([FromHeader(Name = "Authorization")] string token, int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var RemovedCourse = await _courseService.RemoveCourseFromCartByUserIdAsync(userId, courseId);
                return Ok("The course removed from the cart successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

    }









}
