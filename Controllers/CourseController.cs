using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    public class CourseController : Controller
    {
        private readonly SimDbContext _context;

        public CourseController(SimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.OrderBy(c => c.Name).ToListAsync();
            return View(courses);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Major,Duration,Description")] Course course)
        {
            if (await _context.Courses.AnyAsync(c => c.Name == course.Name))
            {
                ModelState.AddModelError("Name", "Tên khóa học này đã tồn tại. Vui lòng chọn tên khác.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Major,Duration,Description")] Course course)
        {
            if (id != course.Id) return NotFound();

            if (ModelState.IsValid)
            {
                if (await _context.Courses.AnyAsync(c => c.Name == course.Name && c.Id != id))
                {
                    ModelState.AddModelError("Name", "Tên khóa học này đã được sử dụng.");
                    return View(course);
                }

                try
                {
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Course updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.Id == course.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
            if (course == null) return NotFound();

            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.CourseId == id);
            if (isAssigned) ViewData["ErrorMessage"] = "This course cannot be deleted because it is assigned.";

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.CourseId == id);
            if (isAssigned)
            {
                TempData["ErrorMessage"] = "Deletion failed. The course is still assigned to a class.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The course has been deleted successfully.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}