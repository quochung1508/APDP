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
                studentsQuery = studentsQuery.Where(s =>
                    s.FullName.Contains(searchString) ||
                    s.StudentNumber.Contains(searchString) ||
                    s.User.Email.Contains(searchString) ||
                    s.PhoneNumber.Contains(searchString)
                );
            }

            ViewData["CurrentFilter"] = searchString;

            var students = await studentsQuery
                .Select(s => new StudentListViewModel
                {
                    UserId = s.UserId,
                    StudentNumber = s.StudentNumber,
                    FullName = s.FullName,
                    DateOfBirth = s.DateOfBirth,
                    Gender = s.Gender,
                    Address = s.Address,
                    PhoneNumber = s.PhoneNumber
                })
                .ToListAsync();

            return View(students);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Schedule()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null)
            {
                ViewData["ErrorMessage"] = "A student profile for your account could not be found.";
                return View(new List<StudentScheduleViewModel>());
            }

            ViewData["StudentName"] = student.FullName;
            ViewData["StudentNumber"] = student.StudentNumber;

            var schedule = await _context.ClassAssignments
                .Where(ca => ca.StudentId == student.Id)
                .Include(ca => ca.Course)
                .Include(ca => ca.Class)
                .Include(ca => ca.Teacher).ThenInclude(t => t.User)
                .Select(ca => new StudentScheduleViewModel
                {
                    CourseName = ca.Course.Name,
                    ClassName = ca.Class.Name,
                    Schedule = ca.Class.Schedule,
                    TeacherName = ca.Teacher != null ? (ca.Teacher.FullName ?? ca.Teacher.User.UserName) : "Not Assigned"
                })
                .ToListAsync();

            return View(schedule);
        }

        [Authorize(Roles = "Student")]
        public async Task<IActionResult> AttendanceReport()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
            if (student == null)
            {
                TempData["ErrorMessage"] = "Student profile not found.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["StudentName"] = student.FullName;

            var records = await _context.Attendances
                .Where(a => a.StudentId == student.Id)
                .Include(a => a.Class)
                .Include(a => a.Course)
                .Include(a => a.Teacher)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new StudentAttendanceReportViewModel.AttendanceRecord
                {
                    AttendanceDate = a.AttendanceDate,
                    ClassName = a.Class.Name,
                    CourseName = a.Course.Name,
                    Status = a.Status,
                    Remarks = a.Remarks,
                    TeacherName = a.Teacher.FullName
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
    }
}
