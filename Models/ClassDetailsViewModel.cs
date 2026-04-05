using System.Collections.Generic;

namespace SIMS.Models
{
    public class ClassDetailsViewModel
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; }
        public string Schedule { get; set; } // Added this property to fix the error

        // Define a nested class to hold detailed student information
        public class StudentInfo
        {
            public string StudentId { get; set; }
            public string FullName { get; set; }
            public string PhoneNumber { get; set; }
        }

        // The Dictionary will now hold a list of StudentInfo objects for each course
        public Dictionary<string, List<StudentInfo>> CourseRoster { get; set; }

        public ClassDetailsViewModel()
        {
            CourseRoster = new Dictionary<string, List<StudentInfo>>();
        }
    }
}
