using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.Models
{
    public class Objective
    {
        [Key]
        public int ObjectiveID { get; set; }

        [Required]
        public string Description { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }

        public virtual Course Course { get; set; }
    }
}
