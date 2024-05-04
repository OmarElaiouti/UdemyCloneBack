using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAL.DTOs.CoursePartsDtos;

namespace Udemy.DAL.DTOs
{
    public class InstructorDashboardDataToPostDto
    {
        public string CategoryName { get; set; }
        public string SubcatName { get; set; }
        public string TopicName { get; set; }
        public string CourseTitle { get; set; }
        public string CourseDisc { get; set; }
        public float Price { get; set; }
        public string Level { get; set; }
        public string Cover { get; set; }
        public string Language { get; set; }

        public IEnumerable<ObjectiveDto> Objectives { get; set; }
        public string FullDescription {  get; set; }
        public SectionWithoutFile Section {  get; set; }
        public IEnumerable<RequirementDto> Requirements {  get; set; }
    }
}
