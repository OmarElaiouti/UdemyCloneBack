using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UdemyUOW.Core.DTOs;

namespace UdemyUOW.Core.Interfaces
{
    public interface ICourseDataRepository 
    {

        Task<CourseDataDto> GetSectionsByCourseIdAsync(int courseId);

    }
}
