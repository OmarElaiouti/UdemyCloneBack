using Udemy.Core.Models;

namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class LessonDto
    {
        public int LessonId { get; set; }

        public string LessonName { get; set; }

        public int LessonTimeInMinutes { get; set; }

        public string LessonVideo { get; set; }

        public bool IsCompleted { get; set; }

        public IEnumerable<NoteDto> Notes { get; set; }
    }

}
