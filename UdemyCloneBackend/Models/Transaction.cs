using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UdemyCloneBackend.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionID { get; set; }

        [ForeignKey("Cart")]
        public int CartID { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public DateTime Date { get; set; }

        public string Status { get; set; }


        // Navigation property to the Order
        public virtual Cart Cart { get; set; }
    }
}
