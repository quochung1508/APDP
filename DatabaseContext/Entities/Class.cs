using System.ComponentModel.DataAnnotations.Schema;

namespace SIMS.DatabaseContext.Entities
{
    public class Class
    {
        [Column("id")]
        public long Id { get; set; }

        public string Name { get; set; } = null!;

        public string Schedule { get; set; } = null!;

        [Column("course_id")]
        public long? CourseId { get; set; }

        // Navigation property
        public virtual Course? Course { get; set; }
    }
}
