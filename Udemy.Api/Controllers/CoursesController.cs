﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Udemy.BLL.Interfaces;
using Udemy.BLL.Services.Interfaces;
using Udemy.DAL.DTOs.CourseDtos;


namespace UdemyApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IUserService _userService;


        public CoursesController(
            ICourseService courseService,
            IUserService userService
            )
        {
            _courseService = courseService;
            _userService = userService;

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
                var courses = await _courseService.SearchCoursesByNameWithObjectivesAsync(searchHitory,5);
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
        [Authorize]
        public async Task<ActionResult<List<CourseCardWithLevelDto>>> GetCoursesInCartByUserId()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var courses = await _userService.GetCoursesInCartByUserId(userId);

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
        [Authorize]
        public async Task<ActionResult<List<CourseLongDto>>> GetCoursesInWishlistlistById()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var courses = await _userService.GetCoursesInWishlistByUserId(userId);

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
        [Authorize]
        public async Task<ActionResult<List<CourseLongDto>>> GetEnrolledInCoursesById()
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var courses = await _userService.GetEnrolledInCoursesByUserId(userId);

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
        [Authorize]
        public async Task<IActionResult> AddCourseToCart(int courseId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var addedCourse = await _userService.AddCourseToCartByUserIdAsync(userId, courseId);
                return Ok(addedCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

        [HttpGet("addCourseToWishlist/{courseId}")]
        [Authorize]
        public async Task<IActionResult> AddCourseToWishlist(int courseId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var addedCourse = await _userService.AddCourseToWishlistByUserIdAsync(userId, courseId);
                return Ok(addedCourse);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        }

        [HttpGet("RemoveCourseFromWishlist/{courseId}")]
        [Authorize]
        public async Task<IActionResult> RemoveCourseFromWishlist(int courseId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var RemovedCourse = await _userService.RemoveCourseFromWishlistByUserIdAsync(userId, courseId);
                return Ok("The course removed from the whishlist successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        }

        [HttpGet("remove-course-from-cart/{courseId}")]
        [Authorize]
        public async Task<IActionResult> RemoveCourseFromCart(int courseId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var RemovedCourse = await _userService.RemoveCourseFromCartByUserIdAsync(userId, courseId);
                return Ok("The course removed from the cart successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex}");
            }
        }

    }









}
