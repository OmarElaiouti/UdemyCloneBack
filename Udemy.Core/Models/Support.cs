using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Support
    {
        [Key]
        public int SupportID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }

        [Required]
        public string IssueDescription { get; set; }

        [Required]
        public DateTime DateSubmitted { get; set; }

        [Required]
        public string Status { get; set; }

        [ForeignKey("Admin")]
        public string? AdminID { get; set; }

        // Navigation property to the Admin
        public virtual User Admin { get; set; }

        // Navigation property to the User who submitted the support request
        public virtual User User { get; set; }
    }
}
