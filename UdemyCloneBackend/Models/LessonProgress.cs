using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;

namespace UdemyCloneBackend.Models
{
    public class LessonProgress
    {
    public int Id { get; set; }
    public bool IsCompleted { get; set; }

    [ForeignKey("Lesson")]
    public int LessonID { get; set; }

    [ForeignKey("Enrollment")]
    public int EnrollmentID { get; set; }
    public virtual Enrollment Enrollment { get; set; }
    public virtual Lesson Lesson { get; set; }
}
}
