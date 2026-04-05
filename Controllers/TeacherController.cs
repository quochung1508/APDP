using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using SIMS.Models;
using SIMS.Models.Teacher;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    [Authorize(Roles = "Teacher,Admin")]
    public class TeacherController : Controller
    {
        private readonly SimDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(SimDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers
                .Select(t => new TeacherListViewModel
                {
                    UserId = t.UserId,
                    TeacherNumber = t.TeacherNumber,
                    FullName = t.FullName
                })
                .ToListAsync();
            return View(teachers);
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Schedule()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Challenge();

            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
            if (teacher == null)
            {
                TempData["ErrorMessage"] = "Teacher profile not found.";
                return RedirectToAction("Index", "Home");
            }

            ViewData["TeacherName"] = teacher.FullName;

            var assignments = await _context.ClassAssignments
                .Where(ca => ca.TeacherId == teacher.Id && ca.ClassId.HasValue)
                .Include(ca => ca.Class)
                .Include(ca => ca.Course)
                .Select(ca => new { ClassId = ca.ClassId.Value, ca.Class.Name, ca.Class.Schedule, CourseName = ca.Course.Name })
                .Distinct()
                .ToListAsync();

            var classDetails = new List<ClassDetail>();
            var studentIds = new HashSet<long>();

            foreach (var assignment in assignments)
            {
                var studentCount = await _context.ClassAssignments
                    .Where(ca => ca.ClassId == assignment.ClassId && ca.StudentId.HasValue)
                    .Select(ca => ca.StudentId)
                    .Distinct()
                    .CountAsync();

                classDetails.Add(new ClassDetail
                {
                    // Applying the explicit cast as requested by the compiler error.
                    ClassId = (int)assignment.ClassId,
                    ClassName = assignment.Name,
                    CourseName = assignment.CourseName,
                    Schedule = assignment.Schedule,
                    StudentCount = studentCount
                });

                var studentsInClass = await _context.ClassAssignments
                    .Where(ca => ca.ClassId == assignment.ClassId && ca.StudentId.HasValue)
                    .Select(ca => ca.StudentId.Value)
                    .ToListAsync();

                foreach (var studentId in studentsInClass)
                {
                    studentIds.Add(studentId);
                }
            }

            var viewModel = new TeacherDashboardViewModel
            {
                TotalClasses = assignments.Count,
                TotalStudents = studentIds.Count,
                AssignedClasses = classDetails.OrderBy(c => c.ClassName).ToList()
            };

            return View(viewModel);
        }
    }
}
