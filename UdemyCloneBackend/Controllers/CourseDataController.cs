using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.DTOs.CoursePartsDtos;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using UdemyCloneBackend.Services;
using YamlDotNet.Core.Tokens;

namespace Udemy.API.Controllers
{
    [Route("api/course-data")]
    [ApiController]
    public class CourseDataController : ControllerBase
    {
        private readonly ICourseDataRepository _courseDataService;
        private readonly IAuthService _authService;

        public CourseDataController(ICourseDataRepository courseDataService, IAuthService authService)
        {
            _courseDataService = courseDataService;
            _authService = authService;
        }

        [HttpGet("api/course-sections/{courseId}")]
        public async Task<ActionResult<CourseSectionsDto>> GetCourseSections([FromHeader(Name = "Authorization")] string token,int courseId)
        {

            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpGet("api/course-announcements/{courseId}")]
        public async Task<ActionResult<IEnumerable<AnnouncmentDto>>> GetCourseAnnouncements([FromHeader(Name = "Authorization")] string token, int courseId)
        {

            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpGet("api/course-reviews/{courseId}")]
        public async Task<ActionResult<IEnumerable<FeedbackDto>>> GetCourseReviews([FromHeader(Name = "Authorization")] string token, int courseId)
        {

            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpGet("api/student-review/{courseId}")]
        public async Task<ActionResult<FeedbackDto>> GetStudentReviewOnCourse([FromHeader(Name = "Authorization")] string token, int courseId)
        {

            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpPost("api/update-student-review/{courseId}")]
        public async Task<ActionResult> SetStudentReviewOnCourse([FromHeader(Name = "Authorization")] string token, int courseId, [FromBody] FeedbackDto feedbackDto)
        {

            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpGet("api/{courseId}/get-q&a")]
        public async Task<ActionResult<IEnumerable<CourseCommentDto>>> GetCommentsOnCourse([FromHeader(Name = "Authorization")] string token, int courseId)
        {
            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpPost("api/{courseId}/update-q&a")]
        public async Task<ActionResult> SetStudentCommentOnCourse([FromHeader(Name = "Authorization")] string token, int courseId, [FromBody] CourseCommentDto comment)
        {

            string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));

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

        [HttpGet("api/{courseId}/get-overview")]
        public async Task<ActionResult<OverviewDto>> GetCourseOverView([FromHeader(Name = "Authorization")] string token, int courseId)
        {
            try
            {
                // Decode and validate the token
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));
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


        [HttpPut("api/{courseId}/set-student-lessons-status")]
        public async Task<ActionResult<bool>> SetStudentLessonsStatus([FromHeader(Name = "Authorization")] string token,int courseId, [FromBody] IEnumerable<LassonStatusDto> lessonStatusDto)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));
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


        [HttpGet("api/{courseId}/get-or-create-certificate")]
        public async Task<ActionResult<CertificateDto>> GetOrCreateCertificateData([FromHeader(Name = "Authorization")] string token, int courseId)
        {
            try
            {
                string userId = await _authService.DecodeTokenAsync(token.Replace("Bearer ", ""));
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
