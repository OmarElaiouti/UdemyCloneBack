using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Udemy.BLL.Interfaces;
using Udemy.BLL.Services.Interfaces;
using Udemy.DAL.DTOs;
using Udemy.DAL.DTOs.CourseDtos;
using Udemy.DAL.DTOs.CoursePartsDtos;


namespace Udemy.API.Controllers
{
    [Route("course-class")]
    [ApiController]
    [Authorize]
    public class CourseDataController : ControllerBase
    {
        private readonly ICourseDataService _courseDataService;

        public CourseDataController(ICourseDataService courseDataService)
        {
            _courseDataService = courseDataService;
        }

        [HttpGet("course-sections/{courseId}")]
        [Authorize]
        public async Task<ActionResult<CourseSectionsDto>> GetCourseSections(int courseId)
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var courseSectionsDto = await _courseDataService.GetSectionsByCourseIdAsync(courseId, userId);

                if (courseSectionsDto == null)
                {
                    return NotFound();
                }
                return Ok(courseSectionsDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }
        

        }


        [HttpGet("course-announcements/{courseId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<AnnouncmentDto>>> GetCourseAnnouncements(int courseId)
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var courseAnnouncements = await _courseDataService.GetAnnouncementByCourseIdAsync(courseId);

                if (courseAnnouncements == null)
                {
                    return NotFound();
                }
                return Ok(courseAnnouncements);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }


        }

        [HttpGet("course-reviews/{courseId}")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetCourseReviews(int courseId)
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var courseReviews = await _courseDataService.GetReviewsByCourseIdAsync(courseId);

                if (courseReviews == null)
                {
                    return NotFound();
                }
                return Ok(courseReviews);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }


        }

        [HttpGet("student-review/{courseId}")]
        [Authorize]
        public async Task<ActionResult<FeedbackDto>> GetStudentReviewOnCourse(int courseId)
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            try
            {
                var StudentReview = await _courseDataService.GetStudentReviewOnCourseByCourseIdAsync(courseId,userId);

                if (StudentReview == null)
                {
                    return NotFound();
                }
                return Ok(StudentReview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"{ex}");
            }


        }

        [HttpPost("update-student-review/{courseId}")]
        [Authorize]
        public async Task<ActionResult> SetStudentReviewOnCourse( int courseId, [FromBody] FeedbackDto feedbackDto)
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            bool result = await _courseDataService.SetStudentReviewOnCourse(courseId, userId, feedbackDto);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to set review on course. Enrollment not found.");
            }


        }

        [HttpGet("{courseId}/get-q&a")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<CourseCommentDto>>> GetCommentsOnCourse(int courseId)
        {
            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            var comments = await _courseDataService.GetCommentsByCourseIdAsync(courseId, userId);

            if (comments == null)
            {
                return NotFound(); // Or return an empty list, depending on your business logic
            }

            return Ok(comments);


        }

        [HttpPost("{courseId}/update-q&a")]
        [Authorize]
        public async Task<ActionResult> SetStudentCommentOnCourse(int courseId, [FromBody] CourseCommentDto comment)
        {

            string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("Invalid token or token expired.");
            }

            bool result = await _courseDataService.SetStudentCommentOnCourse(courseId, userId, comment);

            if (result)
            {
                return Ok();
            }
            else
            {
                return BadRequest("Failed to set comment on course. Enrollment not found.");
            }


        }

        [HttpGet("{courseId}/get-overview")]
        [Authorize]
        public async Task<ActionResult<OverviewDto>> GetCourseOverView(int courseId)
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                // Retrieve the course overview
                var overview = await _courseDataService.GetCourseOverViewById(courseId);

                if (overview != null)
                {
                    return Ok(overview); // Return the course overview if found
                }
                else
                {
                    return NotFound("Course not found."); // Return 404 if course overview is not found
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                return StatusCode(500, "Failed to get course overview."); // Return 500 if an unexpected error occurs
            }
        }


        [HttpPut("{courseId}/set-student-lessons-status")]
        [Authorize]
        public async Task<ActionResult<bool>> SetStudentLessonsStatus(int courseId, [FromBody] IEnumerable<LassonStatusDto> lessonStatusDto)
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var success = await _courseDataService.SetStudentCourseLessonsStatus(courseId, userId, lessonStatusDto);
                if (success)
                {
                    return Ok(success); // or return any appropriate success response
                }
                else
                {
                    return StatusCode(500, "Failed to update lesson progress."); // or return any appropriate failure response
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request."); // or return any appropriate failure response
            }
        }


        [HttpGet("{courseId}/get-or-create-certificate")]
        [Authorize]
        public async Task<ActionResult<CertificateDto>> GetOrCreateCertificateData(int courseId)
        {
            try
            {
                string? userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("Invalid token or token expired.");
                }

                var certificateDto = await _courseDataService.GetOrCreateCertificateDataByUserIdAndCourseId(userId, courseId);
                if (certificateDto != null)
                {
                    return Ok(certificateDto);
                }
                else
                {
                    return NotFound(); // Or return appropriate response if certificate data is not found
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while processing your request."); // Or return appropriate error response
            }
        }







    }
}
