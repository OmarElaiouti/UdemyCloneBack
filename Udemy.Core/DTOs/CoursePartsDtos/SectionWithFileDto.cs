using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class SectionWithFileDto
    {
        public string SectionName { get; set; }

        public IEnumerable<LessonWithFileDto> Lessons { get; set; }
    }
}
