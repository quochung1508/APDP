using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.DatabaseContext.Entities
{
    [Table("attendance")]
    public class Attendance
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("student_id")]
        [ForeignKey("Student")]
        public long StudentId { get; set; }

        [Column("class_id")]
        [ForeignKey("Class")]
        public long ClassId { get; set; }

        [Column("attendance_date")]
        public DateTime AttendanceDate { get; set; }

        [Column("status")]
        [MaxLength(255)]
        public string Status { get; set; }

        [Column("remarks")]
        [MaxLength(255)]
        public string? Remarks { get; set; }

        [Column("teacher_id")]
        [ForeignKey("Teacher")]
        public long TeacherId { get; set; }

        [Column("course_id")]
        [ForeignKey("Course")]
        public long CourseId { get; set; }

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Class Class { get; set; }
        public virtual Teacher Teacher { get; set; }
        public virtual Course Course { get; set; }
    }
}
