using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyUOW.Core.DTOs
{
    public class InstructorcourseDto
    {
        public string InstructorId { get; set; }

        public string CourseName { get; set; }

        public string InstructorName { get; set; }

        public string InstructorImage { get; set;}

        public float Rating { get; set; }

        public int CoursesCount { get; set; }

        public int StudentsCount { get; set; }
    }
}
