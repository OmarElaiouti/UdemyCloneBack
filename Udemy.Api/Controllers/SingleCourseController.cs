using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.Interfaces;
using Udemy.Core.Interfaces.IRepositories;

namespace UdemyApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SingleCourseController : ControllerBase
    {
        private readonly ICourseDataRepository _courseDataService;
        private readonly IAuthService _authService;

        public SingleCourseController(ICourseDataRepository courseDataService, IAuthService authService)
        {
            _courseDataService = courseDataService;
            _authService = authService;
        }


        [HttpGet("{courseId}")]
        public async Task<ActionResult<SingleCourseDto>> GetSingleCourse(int courseId)
        {
            var course = await _courseDataService.GetSingleCourse(courseId);

            if (course == null)
            {
                return NotFound($"Course with ID {courseId} not found");
            }

            return Ok(course);
        }

        [HttpGet("{courseId}/instructor")]
        public async Task<ActionResult<CourseInstructorDto>> GetCourseInstructor(int courseId)
        {
            var instructorDto = await _courseDataService.GetCourseInstructor(courseId);

            if (instructorDto == null)
            {
                return NotFound(); // Return 404 Not Found if instructor not found
            }

            return Ok(instructorDto); // Return 200 OK with the instructor DTO
        }

        [HttpGet("{courseId}/sections")]
        public async Task<ActionResult<CourseSectionsDto>> GetCourseSections(int courseId)
        {
            var sectionsDto = await _courseDataService.GetCourseSections(courseId);

            if (sectionsDto == null)
            {
                return NotFound(); // Return 404 Not Found if sections not found
            }

            return Ok(sectionsDto); // Return 200 OK with the sections DTO
        }

        [HttpGet("{courseId}/related")]
        public async Task<ActionResult<CourseRelatedDto>> GetRelatedCourseData(int courseId)
        {
            var relatedData = await _courseDataService.GetRelatedCourseData(courseId);

            if (relatedData == null)
            {
                return NotFound(); // Or return any appropriate response for no data found
            }

            return Ok(relatedData);
        }
    }
}





