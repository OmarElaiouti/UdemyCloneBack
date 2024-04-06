using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class SectionWithoutFile
    {
        public string SectionName { get; set; }

        public IEnumerable<LessonWithoutFileDto> Lessons { get; set; }
    }
}
