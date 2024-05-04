using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.BLL.Interfaces;
using Udemy.BLL.Services.Interfaces;
using Udemy.CU.Exceptions;
using Udemy.DAL.DTOs;
using UdemyApi.Controllers;

namespace Udemy.API.Controllers
{

    [Route("[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;
        private readonly IInstructorService _instructorService;
        private readonly ILogger<UserController> _logger;
        public CategoryController(ILogger<UserController> logger,
            ICategoryService categoryService,
            IInstructorService instructorService
            )
        {
            _categoryService = categoryService;
            _logger = logger;
            _instructorService = instructorService;


    }

    [HttpGet("get-categories")]
        public ActionResult<IEnumerable<CategoryDto>> GetCategories()
        {
            try
            {
                var categories = _categoryService.GetCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving categories");
                return StatusCode(500, "An error occurred while retrieving categories.");
            }
        }

        [HttpGet("{parentName}/subcategories-or-topics")]
        public IActionResult GetSubCategoriesOrTopicsByParentName(string parentName)
        {
            try
            {
                var subCategoriesOrTopics = _categoryService.GetSubCategoriesOrTopicsByParentName(parentName);
                if (subCategoriesOrTopics == null)
                    return NotFound($"Parent category '{parentName}' not found.");

                return Ok(subCategoriesOrTopics);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving subcategories/topics.");
            }
        }

        [HttpGet("instructors/{categoryName}")]
        public async Task<IActionResult> GetInstructorsByCategoryName(string categoryName)
        {
            try
            {
                var instructors = await _instructorService.GetInstructorsByCategoryName(categoryName);
                _logger.LogInformation("Retrieved instructors by category name successfully.");
                return Ok(instructors);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex.Message);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving instructors by category name.");
                return StatusCode(StatusCodes.Status500InternalServerError, "Error retrieving instructors.");
            }
        }




    }


}
