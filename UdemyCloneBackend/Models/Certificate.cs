using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UdemyCloneBackend.Models
{
    public class Certificate
    {
        [Key]
        public int CertificateID { get; set; }

        [ForeignKey("Enrollment")]
        public int EnrollmentID { get; set; }

        [Required]
        public string CertificateFile { get; set; }

        // Navigation property to the User
        public virtual Enrollment Enrollment { get; set; }

        // Navigation property to the Course
    }
}
