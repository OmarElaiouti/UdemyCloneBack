using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.Interfaces;
using UdemyUOW.Core.Interfaces;

namespace Udemy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseDataController : ControllerBase
    {
        private readonly ICourseDataRepository _sectionRepository;

        public CourseDataController(ICourseDataRepository sectionRepository)
        {
            _sectionRepository = sectionRepository;
        }

        [HttpGet]
        [Route("api/course/{courseId}")]
        public async Task<IActionResult> GetCourseSections(int courseId)
        {
            var courseData = await _sectionRepository.GetSectionsByCourseIdAsync(courseId);

            if (courseData == null)
            {
                return NotFound();
            }

            return Ok(courseData);
        }
    }
}
