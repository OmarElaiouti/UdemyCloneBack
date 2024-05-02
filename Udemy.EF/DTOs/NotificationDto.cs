using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Udemy.DAL.DTOs
{
    public class NotificationDto
    {
        public int ID { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public bool Status { get; set; }

    }
}
