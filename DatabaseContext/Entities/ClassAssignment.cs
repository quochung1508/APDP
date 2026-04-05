using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.DatabaseContext.Entities
{
    [Table("class_assignments")]
    public class ClassAssignment
    {
        public long Id { get; set; }

        [Column("student_id")]
        [ForeignKey("Student")]
        public long? StudentId { get; set; }
        public Student Student { get; set; }

        [Column("teacher_id")]
        [ForeignKey("Teacher")]
        public long? TeacherId { get; set; }
        public Teacher Teacher { get; set; } // Corrected type

        [Column("class_id")]
        [ForeignKey("Class")]
        public long? ClassId { get; set; }
        public Class Class { get; set; }

        [Column("course_id")]
        [ForeignKey("Course")]
        public long? CourseId { get; set; }
        public Course Course { get; set; }
    }
}
