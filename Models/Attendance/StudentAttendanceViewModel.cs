namespace SIMS.Models.Attendance
{
    public class StudentAttendanceViewModel
    {
        public long StudentId { get; set; }
        public string StudentName { get; set; }
        public string StudentNumber { get; set; }

        // The status can be Present, Absent, Late, etc.
        public string Status { get; set; }

        public string? Remarks { get; set; }
    }
}
