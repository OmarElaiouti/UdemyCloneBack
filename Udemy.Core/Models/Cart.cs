using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Cart
    {
        [Key]
        public int CartID { get; set; }

        [Required]
        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }

        public virtual ICollection<Course> CoursesInCart { get; set; }

        public virtual ICollection<Transaction> Transactions { get; set; }


        [ForeignKey("User")]
        public string? UserID { get; set; }

        // Navigation property to the User

        public virtual User? User { get; set; }
    }
}
