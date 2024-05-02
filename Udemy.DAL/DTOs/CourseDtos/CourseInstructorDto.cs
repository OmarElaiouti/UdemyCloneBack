using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs.CourseDtos
{
    public class CourseInstructorDto
    {
        public string InstructorId { get; set; }

        public string Name { get; set; }

        public string Image { get; set; }
        public string HeadLine { get; set; }
        public string Boiography { get; set; }
        public float Rate { get; set; }
        public int CoursesCount { get; set; }
        public int StudentsCount { get; set; }

        public IEnumerable<CourseWithObjectivesDto> Courses { get; set; }

        public int FeedbacksCount { get; set; }

    }
}
