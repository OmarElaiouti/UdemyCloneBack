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
        private readonly ICourseService _courseService;

        public CoursesController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public IActionResult Search([FromQuery] string searchString)
        {
            try
            {
              
                return Ok(_courseService.SearchCourses(searchString));
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500,ex);
            }
        }
    }
}
