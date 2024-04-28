using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models.UdemyContext;
using Udemy.Core.Models;
using Udemy.Core.DTOs.CourseDtos;
using Udemy.Core.DTOs;
using Microsoft.EntityFrameworkCore;
using Udemy.EF.Repository.NewFolder;
using Udemy.EF.UnitOfWork;
using Udemy.Core.Interfaces.IRepositories;

namespace Udemy.EF.Repository
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
