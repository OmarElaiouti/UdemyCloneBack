using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class OverviewDto
    {
        public int CourseID { get; set; }   
        public string BriefDescription { get; set; }
        public string Level { get; set; }
        public int NumStudent { get; set; }
        public int TotalLessons { get; set; }
        public string TotalHours { get; set; }
        public string FullDescription { get; set; }
        public string Language { get; set; }


        public string InstructorName { get; set; }
        public string InstrucHeadlind { get; set; }
        public string InstructorBiography { get; set; }
        public float InstructorRate {  get; set; }
        public string InstructorImage {  get; set; }


        public IEnumerable<ObjectiveDto> Objectives { get; set; }
        public IEnumerable<RequirementDto> Requirements { get; set; }

    }
}
