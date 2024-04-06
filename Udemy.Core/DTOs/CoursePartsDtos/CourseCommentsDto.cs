using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.Core.DTOs.CoursePartsDtos
{
    public class CourseCommentDto
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string StudentName { get; set; }
        public string StudentImage { get; set; }
        public int? AnswerTo { get; set; }
        public bool isReply {  get; set; }
        public int CourseID { get; set; }

        public bool isUserComment { get; set; }
        public bool isUpdated { get; set; }

        public string Date { get; set; }


    }
}
