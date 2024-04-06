using Udemy.Core.Interfaces;
using Udemy.Core.Models;

namespace Udemy.Core.Interfaces
{
    public interface IInstructorRepository : IBaseRepository<User>
    {
        IEnumerable<User> GetInstructors();
        User GetInstructorByCourseId(int courseId);
         int GetStudentsCountForCourse(int courseId);

    }
}
