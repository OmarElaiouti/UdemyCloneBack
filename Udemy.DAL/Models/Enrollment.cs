using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Udemy.DAl.Models
{
    public class Enrollment
    {
        [Key]

        public int Id { get; set; }

        public DateTime EnrollmentDate { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }

        [ForeignKey("Course")]
        public int CourseId { get; set; }

        [ForeignKey("Certificate")]
        public int? CertificateId { get; set; }


        [ForeignKey("Feedback")]
        public int? FeedbackId { get; set; }

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Course Course { get; set; }
        public virtual Certificate? Certificate { get; set; }
        public virtual Feedback? Feedback { get; set; }
        public virtual ICollection<Note>? UserNotes { get; set; } 
        public virtual ICollection<LessonProgress>? UserProgress { get; set; } 
    }
}
