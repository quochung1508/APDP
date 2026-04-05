using System.Collections.Generic;

namespace SIMS.Models.Attendance
{
    public class AttendanceIndexViewModel
    {
        public class AssignedClassCourse
        {
            public long ClassId { get; set; }
            public string ClassName { get; set; }
            public long CourseId { get; set; }
            public string CourseName { get; set; }
        }

        public List<AssignedClassCourse> AssignedClasses { get; set; }

        public AttendanceIndexViewModel()
        {
            AssignedClasses = new List<AssignedClassCourse>();
        }
    }
}
