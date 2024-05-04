using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Udemy.DAl.Models;

namespace Udemy.DAL.Context
{
    public class UdemyContext : IdentityDbContext<User>
    {
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Cart> Carts { get; set; }
        public virtual DbSet<Transaction> Transactions { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Section> Sections { get; set; }
        public virtual DbSet<Lesson> Lessons { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Announcement> Announcements { get; set; }
        public virtual DbSet<Feedback> Feedbacks { get; set; }
        public virtual DbSet<Certificate> Certificates { get; set; }
        public virtual DbSet<Support> Supports { get; set; }
        public virtual DbSet<Notification> Notifications { get; set; }
        public virtual DbSet<Gift> Gifts { get; set; }
        public virtual DbSet<LessonProgress> LessonProgresses { get; set; }
        public virtual DbSet<Enrollment> Enrollments { get; set; }
        public virtual DbSet<Note> Notes { get; set; }
        public virtual DbSet<Requirement> Requirements { get; set; }

        public virtual DbSet<Objective> Objectives { get; set; }



        public UdemyContext(DbContextOptions<UdemyContext> options) : base(options)
        {
        }

        public UdemyContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseLazyLoadingProxies();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Sender)
                .WithMany()
                .HasForeignKey(g => g.SenderID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Receiver)
                .WithMany()
                .HasForeignKey(g => g.ReceiverID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Gift>()
                .HasOne(g => g.GiftedCourse)
                .WithMany()
                .HasForeignKey(g => g.GiftedCourseID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.CreatedCourses)
                .HasForeignKey(c => c.InstructorID)
                .IsRequired();
           
            modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Development", Type = "Main" },
            new Category { Id = 2, Name = "Art", Type = "Main" },
            new Category { Id = 3, Name = "Business", Type = "Main" }
        );

        // Seed subcategories for Development category
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 4, Name = "Programming", Type = "Sub", ParentId = 1 },
            new Category { Id = 5, Name = "Web Development", Type = "Sub", ParentId = 1 }
        );

        // Seed topics for Programming subcategory
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 6, Name = "JavaScript", Type = "Topic", ParentId = 4 },
            new Category { Id = 7, Name = "C#", Type = "Topic", ParentId = 4 }
        );

        // Seed subcategories for Art category
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 8, Name = "Painting", Type = "Sub", ParentId = 2 },
            new Category { Id = 9, Name = "Sculpture", Type = "Sub", ParentId = 2 }
        );

        // Seed topics for Painting subcategory
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 10, Name = "Oil Painting", Type = "Topic", ParentId = 8 },
            new Category { Id = 11, Name = "Watercolor Painting", Type = "Topic", ParentId = 8 }
        );
      


            // Specify ON DELETE NO ACTION for the ParentId foreign key
     

            // Configure the relationship between Category and Courses



            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.NoAction;
            }
        }
    }
}
