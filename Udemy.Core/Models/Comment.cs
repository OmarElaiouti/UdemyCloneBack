using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Udemy.Core.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public bool isReply { get; set; }

        [ForeignKey("Enrollment")]
        public int EnrollmentID { get; set; }

        [ForeignKey("Course")]
        public int CourseID { get; set; }
        public virtual Course Course { get; set; }

        [ForeignKey("AnswerToComment")]
        public int? AnswerTo { get; set; }
        public virtual Comment AnswerToComment { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Content { get; set; }

        public bool isUpdated { get; set; }
        public virtual Enrollment Enrollment { get; set; }


    }
}
