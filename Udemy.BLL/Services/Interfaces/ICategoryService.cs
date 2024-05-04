using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAL.DTOs;

namespace Udemy.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryDto>> GetCategories();
        Task<IEnumerable<CategoryDto>> GetSubCategoriesOrTopicsByParentName(string parentName);
    }
}
