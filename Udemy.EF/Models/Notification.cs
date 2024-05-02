using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.DAl.Models
{
    public class Notification
    {
        [Key]
        public int NotificationID { get; set; }

        [Required]
        public string EventType { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        [Required]
        public bool Status { get; set; }
        // Navigation property to the User
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual User User { get; set; }
    }
}
