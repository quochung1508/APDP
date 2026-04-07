using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using SIMS.Models;
using SIMS.Models.Student;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    [Authorize]
    public class StudentController : Controller
    {
        private readonly SimDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentController(SimDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index(string searchString)
        {
            var studentsQuery = _context.Students.Include(s => s.User).AsQueryable();
            if (!string.IsNullOrEmpty(searchString))
            {
                studentsQuery = studentsQuery.Where(s => s.FullName.Contains(searchString) || s.StudentNumber.Contains(searchString));
            }
            ViewData["CurrentFilter"] = searchString;

            var students = await studentsQuery.Select(s => new StudentListViewModel
            {
                UserId = s.UserId,
                StudentNumber = s.StudentNumber,
                FullName = s.FullName,
                PhoneNumber = s.PhoneNumber
            }).ToListAsync();

            return View(students);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Schedule()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null) return View(new List<StudentScheduleViewModel>());

            ViewData["StudentName"] = student.FullName;
            ViewData["StudentNumber"] = student.StudentNumber;

            var schedule = await _context.ClassAssignments
                .Where(ca => ca.StudentId == student.Id && !string.IsNullOrEmpty(ca.Schedule))
                .Include(ca => ca.Course)
                .Include(ca => ca.Class)
                .Include(ca => ca.Teacher)
                .Select(ca => new StudentScheduleViewModel
                {
                    CourseName = ca.Course != null ? ca.Course.Name : "Chưa rõ",
                    ClassName = ca.Class != null ? ca.Class.Name : "Chưa rõ",
                    Schedule = ca.Schedule,
                    TeacherName = ca.Teacher != null ? ca.Teacher.FullName : "Chưa phân công" // TRÁNH LỖI NULL
                }).ToListAsync();

            return View(schedule);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AttendanceReport()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null) return RedirectToAction("Index", "Home");

            ViewData["StudentName"] = student.FullName;

            var records = await _context.Attendances
                .Where(a => a.StudentId == student.Id).Include(a => a.Class).Include(a => a.Course).Include(a => a.Teacher)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new StudentAttendanceReportViewModel.AttendanceRecord
                {
                    AttendanceDate = a.AttendanceDate,
                    ClassName = a.Class != null ? a.Class.Name : "Chưa rõ",
                    CourseName = a.Course != null ? a.Course.Name : "Chưa rõ",
                    Status = a.Status,
                    Remarks = a.Remarks,
                    TeacherName = a.Teacher != null ? a.Teacher.FullName : "Chưa phân công"
                }).ToListAsync();

            var report = new StudentAttendanceReportViewModel
            {
                Records = records,
                TotalRecords = records.Count,
                PresentCount = records.Count(r => r.Status == "Present"),
                AbsentCount = records.Count(r => r.Status == "Absent"),
                LateCount = records.Count(r => r.Status == "Late"),
                AttendanceRate = records.Any() ? (double)records.Count(r => r.Status == "Present" || r.Status == "Late") / records.Count * 100 : 0
            };

            return View(report);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> ExportScheduleCsv()
        {
            var user = await _userManager.GetUserAsync(User);
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);

            var schedule = await _context.ClassAssignments
                .Where(ca => ca.StudentId == student.Id && !string.IsNullOrEmpty(ca.Schedule))
                .Include(ca => ca.Course).Include(ca => ca.Class).Include(ca => ca.Teacher)
                .Select(ca => new {
                    CourseName = ca.Course != null ? ca.Course.Name : "Chưa rõ",
                    ClassName = ca.Class != null ? ca.Class.Name : "Chưa rõ",
                    Schedule = ca.Schedule,
                    TeacherName = ca.Teacher != null ? ca.Teacher.FullName : "Chưa phân công"
                }).ToListAsync();

            var builder = new System.Text.StringBuilder();
            builder.Append('\uFEFF');
            builder.AppendLine("Môn học (Course),Lớp (Class),Giảng viên (Teacher),Lịch học (Schedule)");

            foreach (var item in schedule.OrderBy(i => i.CourseName))
            {
                builder.AppendLine($"\"{item.CourseName}\",\"{item.ClassName}\",\"{item.TeacherName}\",\"{item.Schedule}\"");
            }
            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"LichHoc_{student.StudentNumber}.csv");
        }
    }
}