using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Udemy.DAL.DTOs;
using Udemy.DAL.Interfaces;
using Udemy.DAL.UdemyContext;
using Udemy.DAL.UnitOfWork;


namespace Udemy.DAl.Repository
{
    public class CategoryRepository : BaseRepository<Category>, ICategoryRepository
    {
        private readonly UdemyContext _context;
        private readonly IUnitOfWork<UdemyContext> _unitOfWork;

        public CategoryRepository(UdemyContext context, IUnitOfWork<UdemyContext> unitOfWork) : base(context)
        {
            _context = context;
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<CategoryDto> GetCategories()
        {
            
            var categories = GetAll(c => c.ParentId == null) // Filter root categories
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return categories;
        }

        public IEnumerable<CategoryDto> GetSubCategoriesOrTopicsByParentName(string parentName)
        {
            var parentCategory = _context.Categories.FirstOrDefault(c => c.Name == parentName);

            if (parentCategory == null)
            {
                return Enumerable.Empty<CategoryDto>();
            }

            var subCategoriesOrTopics = GetAll(c => c.ParentId == parentCategory.Id)
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToList();

            return subCategoriesOrTopics;
        }

        
    }
}
