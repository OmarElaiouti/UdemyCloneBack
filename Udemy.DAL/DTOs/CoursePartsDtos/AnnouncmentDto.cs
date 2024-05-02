using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs.CoursePartsDtos
{
    public class AnnouncmentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string InstructorName { get; set; }
        public string InstructorImage { get; set; }
        public string Date { get; set; }

    }
}
