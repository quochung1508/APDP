using System;
using System.Collections.Generic;

namespace SIMS.Models.Student
{
    public class StudentAttendanceReportViewModel
    {
        public List<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();

        // New statistical properties
        public int TotalRecords { get; set; }
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int LateCount { get; set; }
        public double AttendanceRate { get; set; }

        public class AttendanceRecord
        {
            public DateTime AttendanceDate { get; set; }
            public string ClassName { get; set; }
            public string CourseName { get; set; }
            public string Status { get; set; }
            public string Remarks { get; set; }
            public string TeacherName { get; set; }
        }
    }
}
