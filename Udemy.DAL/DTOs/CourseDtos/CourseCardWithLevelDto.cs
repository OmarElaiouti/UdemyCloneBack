using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs.CourseDtos
{
    public class CourseCardWithLevelDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Level { get; set; }
        public string InstructorName { get; set; }
        public float Rate { get; set; }
        public float Price { get; set; }
        public int TotalLessons { get; set; }
        public string TotalHours { get; set; }
        public int ReviewersNumber { get; set; }


    }
}
