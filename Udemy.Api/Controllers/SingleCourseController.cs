using Microsoft.AspNetCore.Mvc;
using Udemy.BLL.Interfaces;
using Udemy.DAL.DTOs;
using Udemy.DAL.DTOs.CourseDtos;


namespace UdemyApi.Controllers
{
    [Route("courseInfo")]
    [ApiController]
    public class SingleCourseController : ControllerBase
    {
        private readonly ICourseDataService _courseDataService;

        public SingleCourseController(ICourseDataService courseDataService)
        {
            _courseDataService = courseDataService;
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
        public async Task<ActionResult<InstructorDto>> GetCourseInstructor(int courseId)
        {
            var instructorDto = await _courseDataService.GetCourseInstructor(courseId);

            if (instructorDto == null)
            {
                return NotFound(); // Return 404 Not Found if instructor not found
            }

            return Ok(instructorDto); 
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
                return NotFound(); 
            }

            return Ok(relatedData);
        }
    }
}





