using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Udemy.DAL.DTOs.CoursePartsDtos;


namespace Udemy.DAL.DTOs.CourseDtos
{
    public class CourseRelatedDto
    {
        public FeedbackDto FeaturedFeedback { get; set; }

        public IEnumerable<ObjectiveDto> Objectives { get; set; }
        public IEnumerable<RequirementDto> Requirements { get; set; }



    }
}
