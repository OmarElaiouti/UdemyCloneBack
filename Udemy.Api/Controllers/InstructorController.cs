using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Udemy.BLL.Services.Interfaces;
using Udemy.CU.Exceptions;
using Udemy.DAl.Models;

namespace Udemy.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class InstructorController : ControllerBase
    {
        private readonly IInstructorService _instructorService;
        private readonly ILogger<InstructorController> _logger;

        public InstructorController(IInstructorService instructorService, ILogger<InstructorController> logger)
        {
            _instructorService = instructorService;
            _logger = logger;
        }

        [HttpGet("all-instructors")]
        public async Task<IActionResult> GetAllInstructors()
        {
            try
            {
                var instructors = await _instructorService.GetAllInstructors();
                _logger.LogInformation("Retrieved all instructors successfully.");
                return Ok(instructors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all instructors.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving instructors.");
            }
        }

        [HttpGet("{instructorId}")]
        public async Task<IActionResult> GetInstructorById(string instructorId)
        {
            try
            {
                var instructor = await _instructorService.GetInstructorById(instructorId);
                _logger.LogInformation("Retrieved instructor by ID successfully.");
                return Ok(instructor);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instructor by ID.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving instructor.");
            }
        }

        


    }
}
