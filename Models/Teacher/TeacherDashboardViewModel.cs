using System.Collections.Generic;

namespace SIMS.Models.Teacher
{
    public class TeacherDashboardViewModel
    {
        public int TotalClasses { get; set; }
        public int TotalStudents { get; set; }
        public List<ClassDetail> AssignedClasses { get; set; } = new List<ClassDetail>();
    }

    public class ClassDetail
    {
        // Ensuring this is a 'long' to match the database entity.
        public long ClassId { get; set; }
        public string CourseName { get; set; }
        public string ClassName { get; set; }
        public string Schedule { get; set; }
        public int StudentCount { get; set; }
    }
}
