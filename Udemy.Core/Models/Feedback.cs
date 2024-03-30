using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; } // Foreign key
        public virtual Enrollment Enrollment { get; set; } // Navigation property

        [Required]
        public float Rate { get; set; }

        public string Comment { get; set; }

    }
}
