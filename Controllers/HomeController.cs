using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.Models.ViewModels;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    [Authorize] // Ensure only logged-in users can access the dashboard
    public class HomeController : Controller
    {
        private readonly SimDbContext _context;

        public HomeController(SimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Redirect users based on their role to the most relevant page
            if (User.IsInRole("Student"))
            {
                return RedirectToAction("Schedule", "Student");
            }

            if (User.IsInRole("Teacher"))
            {
                return RedirectToAction("Schedule", "Teacher");
            }

            // Admin users will fall through to the main dashboard
            if (User.IsInRole("Admin"))
            {
                var viewModel = new DashboardViewModel
                {
                    TotalStudents = await _context.Students.CountAsync(),
                    TotalCourses = await _context.Courses.CountAsync(),
                    TotalClasses = await _context.Classes.CountAsync(),
                    // Count unique class-course assignments for a more accurate number
                    TotalAssignments = await _context.ClassAssignments.Select(ca => new { ca.ClassId, ca.CourseId }).Distinct().CountAsync()
                };

                return View(viewModel);
            }

            // Fallback for any other authenticated user without a specific role page
            // This could lead to a generic landing page or, in this case, deny access.
            return Challenge(); // Or redirect to a generic landing page
        }
    }
}
