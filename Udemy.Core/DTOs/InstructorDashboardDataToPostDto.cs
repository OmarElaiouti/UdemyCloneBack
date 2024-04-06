using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.Core.DTOs.CoursePartsDtos;

namespace Udemy.Core.DTOs
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
        public IFormFile Cover { get; set; }
        public string Language { get; set; }

        public IEnumerable<ObjectiveDto> Objectives { get; set; }
        public string FullDescription {  get; set; }
        public int Sectionnum {  get; set; }
        public IEnumerable<SectionWithFileDto> Sections {  get; set; }
        public IEnumerable<RequirementDto> Requirements {  get; set; }
    }
}
