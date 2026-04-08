using System.Collections.Generic;

namespace SIMS.Models
{
    public class ClassDetailsViewModel
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; }

        // Đã sửa: Thay Schedule thành Cohort
        public string Cohort { get; set; }

        public string AllEmails { get; set; }

        public class StudentInfo
        {
            public string StudentId { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
            public string Email { get; set; }
        }

        public Dictionary<string, List<StudentInfo>> CourseRoster { get; set; }

        public ClassDetailsViewModel()
        {
            CourseRoster = new Dictionary<string, List<StudentInfo>>();
        }
    }
}