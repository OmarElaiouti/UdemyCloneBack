using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs.CoursePartsDtos
{
    public class FeedbackDto
    {
        public string StudentName {  get; set; }
        public float Rate { get; set; }
        public string StudentImage { get; set; }
        public string Date { get; set; }
        public string ReviewComment { get; set; }



    }
}
