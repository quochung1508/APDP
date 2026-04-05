using System;
using System.Collections.Generic;

namespace SIMS.Models.Attendance
{
    public class TakeAttendanceViewModel
    {
        public long ClassId { get; set; }
        public string ClassName { get; set; }
        public long CourseId { get; set; }
        public string CourseName { get; set; }
        public DateTime AttendanceDate { get; set; }
        public List<StudentAttendanceViewModel> Students { get; set; }

        public TakeAttendanceViewModel()
        {
            Students = new List<StudentAttendanceViewModel>();
            AttendanceDate = DateTime.Today;
        }
    }
}
