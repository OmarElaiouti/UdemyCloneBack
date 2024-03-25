﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Udemy.Core.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }

        [Required]
        public bool Approved { get; set; }

        [Required]
        public DateTime DateCreated { get; set; }

        [Required]
        public decimal Price { get; set; }

        public string Level { get; set; }

        public string BriefDescription { get; set; }

        public string Audience { get; set; }

        public string Objectives { get; set; }

        public string Requirements { get; set; }

        public string FullDescription { get; set; }

        public string Cover { get; set; }

        public string DescVideo { get; set; }


        public string Promo { get; set; }

        public decimal? PromoAmount { get; set; }

        public bool? PromoStatus { get; set; }

        public int? Discount { get; set; }

        public DateTime? DiscountDate { get; set; }



        [ForeignKey("Instructor")]
        public string InstructorID { get; set; }

        public virtual User Instructor { get; set; }

        public virtual ICollection<Cart> Carts { get; set; }
        public virtual ICollection<User> WishListed { get; set; }
        public virtual ICollection<Category> Categories { get; set; }

        public ICollection<Enrollment> Enrollments { get; set; }



    }
}