using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Udemy.Core.Models;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.Repository;
using UdemyUOW.Core.Interfaces;

namespace UdemyUOW.EF.Repository
{
    public class InstructorRepository : BaseRepository<User>, IInstructorRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly UdemyContext _context;

        public InstructorRepository(UserManager<User> userManager, UdemyContext udemyContext) : base(udemyContext)
        {
            _userManager = userManager;
            _context = udemyContext;
        }

        public IEnumerable<User> GetInstructors()
        {
            return _userManager.GetUsersInRoleAsync("Instructor").Result;
        }

        public User GetInstructorByCourseId(int courseId)
        {
            var instructor = _context.Courses
                .Where(c => c.CourseID == courseId)
                .Include(c => c.Instructor)
                .Select(c => c.Instructor)
                .FirstOrDefault();

            if (instructor != null)
            {
                _context.Entry(instructor)
                    .Collection(u => u.CreatedCourses)
                    .Load();

                _context.Entry(instructor)
                    .Collection(u => u.Enrollments)
                    .Load();
            }

            return instructor;
        }

        public int GetStudentsCountForCourse(int courseId)
        {
            var enrollmentsForCourse = _context.Enrollments
                .Where(e => e.CourseId == courseId)
                .ToList(); 

            int studentsCount = enrollmentsForCourse.Count;

            return studentsCount;
        }


    }
}
