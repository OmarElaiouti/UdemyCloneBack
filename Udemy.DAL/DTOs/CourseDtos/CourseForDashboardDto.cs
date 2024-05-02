using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs.CourseDtos
{
    public class CourseForDashboardDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string BriefDescription { get; set; }
        public string InstructorName { get; set; }
        public int ReviewersNumber { get; set; }
        public float Rate { get; set; }
        public float Price { get; set; }
    }
}
