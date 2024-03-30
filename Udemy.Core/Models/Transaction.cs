using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        [ForeignKey("User")]
        public string UserID { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Status { get; set; }


        // Navigation property to the Order
        public virtual User User { get; set; }

        public virtual ICollection<Course> PurchasedCourses { get; set; }

    }
}
