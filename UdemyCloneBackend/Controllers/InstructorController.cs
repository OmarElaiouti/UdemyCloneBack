using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.Models;
using UdemyUOW.Core.DTOs;
using UdemyUOW.Core.Interfaces;

namespace Udemy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InstructorCoursesController : ControllerBase
    {
        private readonly IInstructorRepository _instructorRepository;
        private readonly UserManager<User> _userManager;

        public InstructorCoursesController(IInstructorRepository instructorRepository, UserManager<User> userManager)
        {
            _instructorRepository = instructorRepository;
            _userManager = userManager;
        }

        [HttpGet("instructors")]
        public IActionResult GetAllInstructors()
        {
            var instructors = _instructorRepository.GetInstructors();
            return Ok(instructors);
        }

        [HttpGet("instructor/{courseId}")]
        public IActionResult GetInstructorByCourseId(int courseId)
        {
            var instructor = _instructorRepository.GetInstructorByCourseId(courseId);

            if (instructor == null)
            {
                return NotFound();
            }

            var StudentCount = _instructorRepository.GetStudentsCountForCourse(courseId);
            var instructorDto = new InstructorcourseDto
            {
                InstructorId = instructor.Id,
                CourseName = instructor.CreatedCourses?.FirstOrDefault(c => c.CourseID == courseId)?.Name,
                InstructorName = instructor.UserName,
                InstructorImage = instructor.Image,
                CoursesCount = instructor.CreatedCourses?.Count() ?? 0,
                StudentsCount = StudentCount
            };

            return Ok(instructorDto);
        }



        [HttpGet("studentsCount/{courseId}")]
        public IActionResult GetStudentsCountForCourse(int courseId)
        {

            int studentsCount = _instructorRepository.GetStudentsCountForCourse(courseId);

            return Ok(new { studentsCount });


        }



    }
}
