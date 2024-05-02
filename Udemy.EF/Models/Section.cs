using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.DAl.Models
{
    public class Section
    {
        [Key]
        public int SectionID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        [Required]
        public string Title { get; set; }

        // Navigation property to the Course
        public virtual Course Course { get; set; }

        public virtual ICollection<Lesson> Lessons { get; set; }

    }
}
