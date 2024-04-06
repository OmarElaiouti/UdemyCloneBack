using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        public string Name { get; set; }


        [ForeignKey("ParentCategory")]
        public int? ParentId { get; set; }

        public virtual Category ParentCategory { get; set; }

        public string Type { get; set; }

        public int? Score { get; set; }

        public virtual ICollection<Category> Subcategories { get; set; }

        public virtual ICollection<Course>? Courses { get; set; }

    }
}
