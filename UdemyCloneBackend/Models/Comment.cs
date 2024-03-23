using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace UdemyCloneBackend.Models
{
    public class Comment
    {
        [Key]
        public int CommentID { get; set; }

        [Required]
        public string Type { get; set; }

        [ForeignKey("Enrollment")]
        public int EnrollmentID { get; set; }
        public virtual Enrollment Enrollment { get; set; }

        [ForeignKey("Lesson")]
        public int? LessonID { get; set; }
        public virtual Lesson Lesson { get; set; }

        [ForeignKey("AnswerToComment")]
        public int? AnswerTo { get; set; }
        public virtual Comment AnswerToComment { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Content { get; set; }
    }
}
