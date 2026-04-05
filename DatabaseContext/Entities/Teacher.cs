using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.DatabaseContext.Entities
{
    [Table("teachers")]
    public class Teacher
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("user_id")]
        [ForeignKey("User")]
        public long UserId { get; set; }

        [Column("teacher_number")]
        public string TeacherNumber { get; set; }

        [Column("full_name")]
        public string FullName { get; set; }

        [Column("date_of_birth")]
        public DateTime? DateOfBirth { get; set; }

        [Column("gender")]
        public string? Gender { get; set; }

        [Column("address")]
        public string? Address { get; set; }

        [Column("phone_number")]
        public string? PhoneNumber { get; set; }

        public virtual ApplicationUser User { get; set; }
    }
}
