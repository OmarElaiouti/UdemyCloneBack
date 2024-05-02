using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.DAl.Models
{
    public class Gift
    {
        [Key]
        public int GiftID { get; set; }

        [ForeignKey("SenderStudent")]
        public string SenderID { get; set; }

        [ForeignKey("ReceiverStudent")]
        public string ReceiverID { get; set; }

        [ForeignKey("GiftedCourse")]
        public int GiftedCourseID { get; set; }

        [Required]
        public DateTime Timestamp { get; set; }

        // Navigation properties to related entities
        public virtual User Sender { get; set; }

        public virtual User Receiver { get; set; }

        public virtual Course GiftedCourse { get; set; }
    }
}
