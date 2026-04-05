using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using SIMS.Models.Attendance;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class AttendanceController : Controller
    {
        private readonly SimDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AttendanceController(SimDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Attendance
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);

            if (teacher == null) return Unauthorized("User is not registered as a teacher.");

            var assignments = await _context.ClassAssignments
                .Where(ca => ca.TeacherId == teacher.Id && ca.ClassId.HasValue && ca.CourseId.HasValue)
                .Include(ca => ca.Class)
                .Include(ca => ca.Course)
                .OrderBy(ca => ca.Class.Name).ThenBy(ca => ca.Course.Name)
                .Select(a => new { a.ClassId, ClassName = a.Class.Name, a.CourseId, CourseName = a.Course.Name })
                .Distinct()
                .ToListAsync();

            var viewModel = new AttendanceIndexViewModel();
            viewModel.AssignedClasses = assignments.Select(a => new AttendanceIndexViewModel.AssignedClassCourse
            {
                ClassId = a.ClassId.Value,
                ClassName = a.ClassName,
                CourseId = a.CourseId.Value,
                CourseName = a.CourseName
            }).ToList();

            return View(viewModel);
        }

        // GET: Attendance/Take?classId=1&courseId=2&attendanceDate=2025-01-01
        public async Task<IActionResult> Take(long classId, long courseId, DateTime? attendanceDate)
        {
            var @class = await _context.Classes.FindAsync(classId);
            var course = await _context.Courses.FindAsync(courseId);
            if (@class == null || course == null) return NotFound();

            var date = attendanceDate ?? DateTime.Today;

            var studentAssignments = await _context.ClassAssignments
                .Where(ca => ca.ClassId == classId && ca.CourseId == courseId && ca.StudentId != null)
                .Include(ca => ca.Student)
                .OrderBy(ca => ca.Student.FullName)
                .ToListAsync();

            var existingAttendances = await _context.Attendances
                .Where(a => a.ClassId == classId && a.CourseId == courseId && a.AttendanceDate.Date == date.Date)
                .ToDictionaryAsync(a => a.StudentId);

            var viewModel = new TakeAttendanceViewModel
            {
                ClassId = classId,
                ClassName = @class.Name,
                CourseId = courseId,
                CourseName = course.Name,
                AttendanceDate = date
            };

            foreach (var assignment in studentAssignments)
            {
                if (assignment.Student != null)
                {
                    existingAttendances.TryGetValue(assignment.StudentId.Value, out var existingRecord);

                    viewModel.Students.Add(new StudentAttendanceViewModel
                    {
                        StudentId = assignment.StudentId.Value,
                        StudentName = assignment.Student.FullName,
                        StudentNumber = assignment.Student.StudentNumber,
                        Status = existingRecord?.Status ?? "Present",
                        Remarks = existingRecord?.Remarks
                    });
                }
            }

            return View(viewModel);
        }

        // POST: Attendance/Submit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(TakeAttendanceViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Take", model);
            }

            var user = await _userManager.GetUserAsync(User);
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (teacher == null) return Unauthorized("User is not a registered teacher.");

            foreach (var studentAttendance in model.Students)
            {
                var existingRecord = await _context.Attendances.FirstOrDefaultAsync(a =>
                    a.StudentId == studentAttendance.StudentId &&
                    a.ClassId == model.ClassId &&
                    a.CourseId == model.CourseId &&
                    a.AttendanceDate.Date == model.AttendanceDate.Date);

                if (existingRecord != null)
                {
                    existingRecord.Status = studentAttendance.Status;
                    existingRecord.Remarks = studentAttendance.Remarks;
                    _context.Attendances.Update(existingRecord);
                }
                else
                {
                    var newRecord = new Attendance
                    {
                        StudentId = studentAttendance.StudentId,
                        ClassId = model.ClassId,
                        CourseId = model.CourseId,
                        TeacherId = teacher.Id,
                        AttendanceDate = model.AttendanceDate,
                        Status = studentAttendance.Status,
                        Remarks = studentAttendance.Remarks
                    };
                    _context.Attendances.Add(newRecord);
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Attendance has been successfully recorded/updated!";
            return RedirectToAction(nameof(Index));
        }
    }
}
