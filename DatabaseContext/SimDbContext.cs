using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext.Entities;

namespace SIMS.DatabaseContext
{
    public class SimDbContext : IdentityDbContext<ApplicationUser, IdentityRole<long>, long>
    {
        public SimDbContext(DbContextOptions<SimDbContext> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<ClassAssignment> ClassAssignments { get; set; }
        public DbSet<Class> Classes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Attendance> Attendances { get; set; } // Added Attendance DbSet
        public DbSet<ApplicationUser> ApplicationUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Student)
                .WithOne(s => s.User)
                .HasForeignKey<Student>(s => s.UserId);

            builder.Entity<ApplicationUser>()
                .HasOne(a => a.Teacher)
                .WithOne(t => t.User)
                .HasForeignKey<Teacher>(t => t.UserId);

            // ClassAssignment Deletion Rules
            builder.Entity<ClassAssignment>()
                .HasOne(ca => ca.Student)
                .WithMany()
                .HasForeignKey(ca => ca.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClassAssignment>()
                .HasOne(ca => ca.Teacher)
                .WithMany()
                .HasForeignKey(ca => ca.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClassAssignment>()
                .HasOne(ca => ca.Class)
                .WithMany()
                .HasForeignKey(ca => ca.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<ClassAssignment>()
                .HasOne(ca => ca.Course)
                .WithMany()
                .HasForeignKey(ca => ca.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance Deletion Rules (following the same cascade pattern)
            builder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany()
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.Teacher)
                .WithMany()
                .HasForeignKey(a => a.TeacherId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.Class)
                .WithMany()
                .HasForeignKey(a => a.ClassId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasOne(a => a.Course)
                .WithMany()
                .HasForeignKey(a => a.CourseId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
