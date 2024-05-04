using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAl.Models;
using Udemy.DAL.DTOs.CourseDtos;


namespace Udemy.BLL.Interfaces
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseLongDto>> SearchCoursesByNameAsync(string searchString);
        Task<IEnumerable<CourseWithObjectivesDto>> SearchCoursesByNameWithObjectivesAsync(string searchString, int count);
        Task<IEnumerable<CourseWithObjectivesDto>> GetRandomCourses(int count);
        Task<IEnumerable<CourseWithObjectivesDto>> GetTopRatedCourses(int count);
        Task<IEnumerable<CourseLongDto>> GetCoursesByCategory(string categoryName, int? count);
        Task<IEnumerable<CourseCardWithLevelDto>> GetCoursesByIds(List<int> itemIds);
        Task<IEnumerable<CourseWithObjectivesDto>> GetCoursesByCategoryWithObjctives(string categoryName, int? count);

    }
}
