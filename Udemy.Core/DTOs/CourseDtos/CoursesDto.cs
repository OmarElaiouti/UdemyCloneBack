using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CourseDtos
{
    public class CoursesDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Image { get; set; }
        public string Instructor { get; set; }
        public float Rating { get; set; }
        public float Price { get; set; }
        public int TotalLectures { get; set; }
        public int TotalHours { get; set; }
    }
}
