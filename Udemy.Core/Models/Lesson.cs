using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Lesson
    {
        [Key]
        public int LessonID { get; set; }

        [Required]
        public string Title { get; set; }

        [ForeignKey("Section")]
        public int SectionID { get; set; }


        [ForeignKey("LessonProgress")]
        public int LessonProgressID { get; set; }

        // Resources might be a collection of URLs, so adjust the type accordingly
        public string Resources { get; set; }

        // File path or URL to the content file
        public string File { get; set; }

        public int Duration { get; set; }

        // Type of content (e.g., video, text, audio, etc.)
        public string Type { get; set; }

        // Whether the content is allowed or restricted
        public bool Allowed { get; set; }

        // Navigation property to the Section
        public virtual Section Section { get; set; }

        public virtual LessonProgress LessonProgress { get; set; }

        public virtual ICollection<Note> Notes { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

    }
}
