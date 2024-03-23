using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UdemyCloneBackend.Models
{
    public class Announcement
    {
        [Key]
        public int AnnouncementID { get; set; }

        public List<string> Image { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime DatePosted { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        [Required]
        public string Title { get; set; }

        // Navigation property to the Course
        public virtual Course Course { get; set; }
    }
}
