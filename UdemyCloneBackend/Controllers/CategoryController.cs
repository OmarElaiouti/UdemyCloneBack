using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.DTOs;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;

namespace Udemy.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryService;

        public CategoryController(ICategoryRepository categoryService)
        {
            _categoryService = categoryService;

        }

        [HttpGet("get-categories")]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            try
            {
                var categories = await _categoryService.GetCategories();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                // Log the exception
                return StatusCode(500, "An error occurred while retrieving categories.");
            }
        }

        [HttpGet("{parentName}/subcategories-or-topics")]
        public async Task<IActionResult> GetSubCategoriesOrTopicsByParentName(string parentName)
        {
            try
            {
                var subCategoriesOrTopics = await _categoryService.GetSubCategoriesOrTopicsByParentName(parentName);
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




    }


}
