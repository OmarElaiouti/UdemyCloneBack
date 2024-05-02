using System.ComponentModel.DataAnnotations.Schema;

namespace Udemy.DAl.Models
{
    public class Note
    {

        public int NoteId { get; set; }
        public string Content { get; set; }

        [ForeignKey("Enrollment")]
        public int EnrollmentId { get; set; } // Foreign key
        public virtual Enrollment Enrollment { get; set; } // Navigation property


        [ForeignKey("Lesson")]
        public int LessonID { get; set; } // Foreign key
        public Lesson Lesson { get; set; } // Navigation property
    }

}
