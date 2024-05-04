using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAL.DTOs.CoursePartsDtos;


namespace Udemy.DAL.DTOs
{
    public class InstructorDashboardDataToGetDto
    {
        public string CategoryName { get; set; }
        public string SubcatName { get; set; }
        public string TopicName { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDisc { get; set; }
        public float Price { get; set; }
        public string Level { get; set; }
        public string Cover { get; set; }
        public string DescVideo { get; set; }

        public string Language { get; set; }

        public IEnumerable<ObjectiveDto> Objectives { get; set; }
        public string FullDescription { get; set; }
        public int Sectionnum { get; set; }
        public IEnumerable<SectionWithoutFile> Sections { get; set; }
        public IEnumerable<RequirementDto> Requirements { get; set; }
    }
}
