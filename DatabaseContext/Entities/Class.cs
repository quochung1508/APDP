using System.Collections.Generic;

namespace SIMS.DatabaseContext.Entities
{
    public class Class
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;

        // MỚI: Khóa học (VD: Khóa 17, Khóa 18)
        public string? Cohort { get; set; }

        public virtual ICollection<ClassAssignment> ClassAssignments { get; set; } = new List<ClassAssignment>();
    }
}