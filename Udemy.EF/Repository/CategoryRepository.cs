using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Interfaces;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Models;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Udemy.EF.Repository
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly IBaseRepository<Category> _categoryService;
        private readonly UdemyContext _dbcontext;

        public CategoryRepository(IBaseRepository<Category> categoryService, UdemyContext dbContext) : base(dbContext)
        {
            _categoryService = categoryService;
            _dbcontext = dbContext;
        }


        public async Task<IEnumerable<CategoryDto>> GetCategories()
        {
            var categories = await _dbcontext.Categories
                .Where(c => c.ParentId == null) // Filter root categories (no parent)
                .Select(c => new CategoryDto
                {
                    Id = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();

            return categories;
        }

        public async Task<IEnumerable<CategoryDto>> GetSubCategoriesOrTopicsByParentName(string parentName)
        {
            // Find the parent category by name
            var parentCategory = await _dbcontext.Categories
                .FirstOrDefaultAsync(c => c.Name == parentName);

            if (parentCategory == null)
            {
                // Return empty collection or handle the case when the parent category doesn't exist
                return Enumerable.Empty<CategoryDto>();
            }

            // Query subcategories of the parent category
            var subCategoriesOrTopics = await _dbcontext.Categories
                .Where(c => c.ParentId == parentCategory.CategoryId)
                .Select(c => new CategoryDto
                {
                    Id = c.CategoryId,
                    Name = c.Name
                })
                .ToListAsync();

            return subCategoriesOrTopics;
        }




    }
}
