using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UdemyUOW.Core.DTOs
{
    public class CategoryDto
    {
        public int? ParentId { get; set; }
        public string Name { get; set; }
    }
}
