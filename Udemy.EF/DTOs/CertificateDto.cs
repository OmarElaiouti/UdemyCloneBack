using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs
{
    public class CertificateDto
    {
        public string StudentName { get; set; }
        public string StudentImage { get; set; }

        public string CourseName { get; set; }
        public string InstructorName { get; set; }

        public float Rate { get; set; }
        public string Date { get; set; }
        public int TotalLectures { get; set; }
        public string TotalTime { get; set; }



    }
}
