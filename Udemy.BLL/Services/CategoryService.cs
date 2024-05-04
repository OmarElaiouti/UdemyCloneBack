using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Udemy.DAL.DTOs;
using Udemy.DAL.Context;
using Udemy.DAL.UnitOfWork;
using Udemy.DAl.Models;
using Udemy.DAL.GenericBaseRepository.BaseRepository;
using AutoMapper;
using Udemy.BLL.Interfaces;


namespace Udemy.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly UdemyContext _context;
        private readonly IBaseRepository<Category> _categoryRepository;


        public CategoryService(IBaseRepository<Category> categoryRepository,
            UdemyContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetCategories()
        {

            var categories = await _categoryRepository.GetAllAsync(c => c.ParentId == null); // Filter root categories

            var categoriesDto = _mapper.Map<IEnumerable<CategoryDto>>(categories);

            return categoriesDto;
            
        }

        public async Task<IEnumerable<CategoryDto>>  GetSubCategoriesOrTopicsByParentName(string parentName)
        {
            var parentCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Name == parentName);

            if (parentCategory == null)
            {
                return Enumerable.Empty<CategoryDto>();
            }

        

            var subCategoriesOrTopics = await _categoryRepository.GetAllAsync(c => c.ParentId == null); 

            var subCategoriesOrTopicsDto = _mapper.Map<IEnumerable<CategoryDto>>(subCategoriesOrTopics);

            return subCategoriesOrTopicsDto;

        }

        
    }
}
