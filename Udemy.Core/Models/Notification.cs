using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
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
        public string Status { get; set; }

        // Navigation property to the User
        public virtual ICollection<User> Users { get; set; }
    }
}
