using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAL.DTOs;

namespace Udemy.DAL.Interfaces
{
    public interface ICategoryRepository
    {
        IEnumerable<CategoryDto> GetCategories();
        IEnumerable<CategoryDto> GetSubCategoriesOrTopicsByParentName(string parentName);
    }
}
