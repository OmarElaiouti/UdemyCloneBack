using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class LessonWithoutFileDto
    {
        public string LessonName { get; set; }

        public int LessonTimeInMinutes { get; set; }

    }
}
