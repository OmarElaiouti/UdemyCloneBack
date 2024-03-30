namespace UdemyUOW.Core.DTOs
{
    public class CourseDataDto
    {
        public int CourseId { get; set; }
        public IEnumerable<SectionDto> Sections { get; set; }


    }
  
}
