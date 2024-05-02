using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs.CoursePartsDtos
{
    public class LessonWithFileDto
    {
        public string LessonName { get; set; }

        public int LessonTimeInMinutes { get; set; }

        public IFormFile? LessonVideo { get; set; } 
    }
}
