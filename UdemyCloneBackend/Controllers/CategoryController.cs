using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Udemy.Core.Interfaces;
using Udemy.Core.Models;
using UdemyUOW.Core.DTOs;
using UdemyUOW.Core.Interfaces;

namespace Udemy.API.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly IBaseRepository<Category> _repository;

        public CategoryController(IBaseRepository<Category> repository)
        {
            _repository = repository;

        }

        [HttpGet("Category")]
        public ActionResult<IEnumerable<Category>> GetCategoriesWithNullParent()
        {
            var parentCategorydto = new List<CategoryDto>();


            foreach (var category in _repository.GetAll().Where(c => c.ParentId == null))
            {
                var parentDto = new CategoryDto
                {
                    ParentId = category.CategoryId,
                    Name = category.Name,
                };
                parentCategorydto.Add(parentDto);
            }

            return Ok(parentCategorydto);
        }

        [HttpGet("SubCategory")]
        public ActionResult<IEnumerable<Category>> GetSubCategory(int id)
        {
            var categoryDto = new List<CategoryDto>();

            var sub = _repository.GetById(id, c => c.ParentId == id);

            foreach (var category in sub)
            {
                var subcategory = new CategoryDto
                {
                    ParentId = category.CategoryId,
                    Name = category.Name,
                };

                categoryDto.Add(subcategory);
            }
            return Ok(categoryDto);
        }

        //[HttpGet("Topic")]
        //public ActionResult<IEnumerable<CategoryDto>> GetTopic(int id)
        //{
        //    var categoryDtoList = new List<CategoryDto>();

        //    var topics = _repository.GetEntitiesByCondition(
        //        filterCondition: category => category.ParentId == id,
        //        orderBy: c => c.Score descending
        //    );

        //    foreach (var topic in topics)
        //    {
        //        var categoryDto = new CategoryDto
        //        {
        //            ParentId = topic.CategoryId, 
        //            Name = topic.Name
        //        };

        //        categoryDtoList.Add(categoryDto);
        //    }

        //    return Ok(categoryDtoList);
        //}



    }


}
