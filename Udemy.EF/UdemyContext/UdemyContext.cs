using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Udemy.Core.Models.UdemyContext
{
    public class UdemyContext : IdentityDbContext<User>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Section> Sections { get; set; }
        public DbSet<Lesson> Lessons { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Announcement> Announcements { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }
        public DbSet<Certificate> Certificates { get; set; }
        public DbSet<Support> Supports { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Gift> Gifts { get; set; }
        public DbSet<LessonProgress> LessonProgresses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Note> Notes { get; set; }


        public UdemyContext(DbContextOptions<UdemyContext> options) : base(options)
        {
        }

        public UdemyContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Sender)
                .WithMany()
                .HasForeignKey(g => g.SenderID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gift>()
                .HasOne(g => g.Receiver)
                .WithMany()
                .HasForeignKey(g => g.ReceiverID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Gift>()
                .HasOne(g => g.GiftedCourse)
                .WithMany()
                .HasForeignKey(g => g.GiftedCourseID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Instructor)
                .WithMany(u => u.CreatedCourses)
                .HasForeignKey(c => c.InstructorID)
                .IsRequired();
            modelBuilder.Entity<User>()
            .HasMany(u => u.Followers)
            .WithMany(u => u.Following)
            .UsingEntity<Dictionary<string, object>>(
                "Subscription",
                j => j.HasOne<User>().WithMany().HasForeignKey("SubscriberUserId"),
                j => j.HasOne<User>().WithMany().HasForeignKey("SubscribedToUserId")
            );

            //        modelBuilder.Entity<User>()
            //.HasMany(u => u.WishList)
            //.WithMany(c => c.WishListed)
            //.UsingEntity<Dictionary<string, object>>(
            //    "WishList",
            //    j => j
            //        .HasOne<Course>()
            //        .WithMany()
            //        .HasForeignKey("CourseID")
            //        .OnDelete(DeleteBehavior.NoAction), // Specify the cascade behavior here
            //    j => j
            //        .HasOne<User>()
            //        .WithMany()
            //        .HasForeignKey("UserID")
            //        .OnDelete(DeleteBehavior.NoAction) // Specify the cascade behavior here
            //);
            //        modelBuilder.Entity<Course>()
            //.HasOne(c => c.Instructor)
            //.WithMany(i => i.CreatedCourses)
            //.HasForeignKey(c => c.InstructorID)
            //.OnDelete(DeleteBehavior.NoAction); // Specify the cascade behavior here

            //        modelBuilder.Entity<Enrollment>()
            //.HasOne(e => e.User)
            //.WithMany(u => u.Enrollments)
            //.HasForeignKey(e => e.UserId)
            //.OnDelete(DeleteBehavior.NoAction); // Specify the cascade behavior here

            //        modelBuilder.Entity<Lesson>()
            //.HasOne(l => l.Section)
            //.WithMany(s => s.Lessons)
            //.HasForeignKey(l => l.SectionID)
            //.OnDelete(DeleteBehavior.NoAction);

            //        modelBuilder.Entity<Note>()
            //.HasOne(n => n.Lesson)
            //.WithMany(l => l.Notes)
            //.HasForeignKey(n => n.LessonID)
            //.OnDelete(DeleteBehavior.Restrict);

            foreach (var relationship in modelBuilder.Model.GetEntityTypes().SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }
    }
}
