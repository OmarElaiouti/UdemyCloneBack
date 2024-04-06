using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CourseDtos
{
    public class SingleCourseDto
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string BriefDescription { get; set; }
        public string Category { get; set; }
        public string SubCategoey { get; set; }
        public string Topic { get; set; }
        public float Price { get; set; }
        public string TotalMinutes { get; set; }
        public int TotalLessons { get; set; }
        public int TotalSections { get; set; }
        public float Rate { get; set; }
        public int NumFeedback { get; set; }
        public int NumStudents { get; set; }
        public string IntroVideo { get; set; }

        public string LastUpdateDate { get; set; }
        public string FullDescribtion { get; set; }



    }
}
