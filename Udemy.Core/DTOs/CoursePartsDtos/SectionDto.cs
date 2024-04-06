namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class SectionDto
    {

        public int SectionId { get; set; }

        public string SectionName { get; set; }

        public int TotalLessons { get; set; }

        public string TotalMinutes { get; set; }

        public IEnumerable<LessonDto> Lessons { get; set; }
    }

}
