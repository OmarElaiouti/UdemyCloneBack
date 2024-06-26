﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Udemy.DAl.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public string? Headline { get; set; }

        public string? Image { get; set; }

        public string? Biography { get; set; }

        public virtual ICollection<Course> CreatedCourses { get; set; }

        public virtual ICollection<Course>? WishList { get; set; }

        [ForeignKey("Cart")]

        public int? CartID { get; set; }


        public virtual Cart? Cart { get; set; }


        public virtual ICollection<User>? Followers { get; set; }

        // Navigation property for following (subscribed)
        public virtual ICollection<User>? Following { get; set; }

        public virtual ICollection<Enrollment>? Enrollments { get; set; }

        public virtual ICollection<Notification>? Notifications { get; set; }

        public virtual ICollection<Transaction>? Transactions { get; set; }

    }
}
