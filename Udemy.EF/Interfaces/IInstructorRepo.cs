

using Udemy.DAl.Models;
using Udemy.DAL.BaseRepository;

namespace Udemy.DAL.Interfaces
{
    public interface IInstructorRepository : IBaseRepository<User>
    {
        IEnumerable<User> GetInstructors();
        User GetInstructorByCourseId(int courseId);
         int GetStudentsCountForCourse(int courseId);

    }
}
