using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;

namespace Udemy.DAl.Models
{
    public class Requirement
    {
        [Key]
        public int RequirementID { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        public virtual Course Course { get; set; }
    }
}
