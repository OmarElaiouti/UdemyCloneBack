using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.Models;

namespace Udemy.Core.DTOs
{
    public class SearchCourseDto
    {
        public string Name { get; set; }
        public string BriefDescription { get; set; }
        public string InstructorName { get; set; }
        public float Rate { get; set; }
        public float Price { get; set; }
        public int TotalLessons { get; set; }
        public float TotalHours { get; set; }

    }
}
