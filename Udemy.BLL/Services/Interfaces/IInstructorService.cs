

using Udemy.DAl.Models;
using Udemy.DAL.DTOs;

namespace Udemy.BLL.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<InstructorWithHisCoursesDto> GetInstructorById(string instructorId);
        Task<IEnumerable<InstructorDto>> GetAllInstructors();
        Task<IEnumerable<InstructorDto>> GetInstructorsByCategoryName(string categoryName);

    }
}
