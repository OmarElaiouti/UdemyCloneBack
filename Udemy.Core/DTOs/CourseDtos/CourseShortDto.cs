using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CourseDtos
{
    public class CourseShortDto
    {
        public int ID { get; set; }
        public string Image { get; set; }
        public string Name { get; set; }
        public string InstructorName { get; set; }
        public float Price { get; set; }
        public float Rate { get; set; }

    }
}
