using Microsoft.EntityFrameworkCore;
using Udemy.Core.Models.UdemyContext;
using Udemy.EF.Repository;
using UdemyUOW.Core.DTOs;
using UdemyUOW.Core.Interfaces;

namespace UdemyUOW.EF.Repository
{


    public class CourseDataRepository : BaseRepository<UdemyContext>, ICourseDataRepository
    {
        private readonly UdemyContext _context;

        public CourseDataRepository(UdemyContext context) : base(context)
        {
            _context = context;

        }

       

        public async Task<CourseDataDto> GetSectionsByCourseIdAsync(int courseId)
        {
            var sections = await _context.Sections
                .Include(s => s.Lessons)
                .Where(s => s.CourseID == courseId)
                .ToListAsync();

            var courseDataDto = new CourseDataDto
            {
                CourseId = courseId,
                Sections = sections.Select(s => new SectionDto
                {
                    SectionId = s.SectionID,
                    SectionName = s.Title,
                    Lessons = s.Lessons.Select(l => new LessonDto
                    {
                        LessonId = l.LessonID,
                        LessonName = l.Title,
                        LessonTime = l.Duration
                    }).ToList()
                }).ToList()
            };

            return courseDataDto;

        }

      
    }
}
