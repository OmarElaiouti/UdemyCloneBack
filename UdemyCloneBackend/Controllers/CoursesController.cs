using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;

namespace UdemyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesController : ControllerBase
    {
        private readonly ICourseRepository _courseService;

        public CoursesController(ICourseRepository courseService)
        {
            _courseService = courseService;
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
                return StatusCode(500, $"An error occurred: {ex.Message}");
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

        [HttpGet("course-in-cart")]
        public async Task<ActionResult<List<CourseCardWithRateDto>>> GetCoursesInCartByUserId([FromQuery] string id)
        {
            try
            {
                var courses = await _courseService.GetCoursesInCartByUserId(id);
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










    }
}
