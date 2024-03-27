using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs;

namespace Udemy.Core.Interfaces
{
    public interface ICourseService
    {
        List<SearchCourseDto> SearchCourses(string searchString);

    }
}
