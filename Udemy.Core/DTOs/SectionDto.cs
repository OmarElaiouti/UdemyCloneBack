namespace UdemyUOW.Core.DTOs
{
    public class SectionDto
    {

        public int SectionId { get; set; }

        public string SectionName { get; set; }

        public int ContentLength { get; set; }

        public IEnumerable<LessonDto> Lessons { get; set; } 
    }
  
}
