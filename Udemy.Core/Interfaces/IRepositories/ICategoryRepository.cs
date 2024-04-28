using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;

namespace Udemy.Core.Interfaces.IRepositories
{
    public interface ICategoryRepository
    {
        IEnumerable<CategoryDto> GetCategories();
        IEnumerable<CategoryDto> GetSubCategoriesOrTopicsByParentName(string parentName);
    }
}
