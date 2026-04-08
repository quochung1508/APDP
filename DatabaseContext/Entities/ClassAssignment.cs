using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.DatabaseContext.Entities
{
    public class ClassAssignment
    {
        public long Id { get; set; }

        public long? ClassId { get; set; }
        public long? CourseId { get; set; }
        public long? TeacherId { get; set; }
        public long? StudentId { get; set; }

        // MỚI: Thời khóa biểu chuyển về đây
        public string? Schedule { get; set; }

        [ForeignKey("ClassId")]
        public virtual Class? Class { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course? Course { get; set; }

        [ForeignKey("TeacherId")]
        public virtual Teacher? Teacher { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }
    }
}