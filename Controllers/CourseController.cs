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

        public IActionResult Create()
        {
            return View();
        }

        // POST: Course/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name")] Course course)
        {
            // Kiểm tra: Tên khóa học đã tồn tại chưa?
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
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            return View(course);
        }

        // POST: Course/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name")] Course course)
        {
            if (id != course.Id)
            {
                return NotFound();
            }

            // Kiểm tra tính hợp lệ và kiểm tra trùng tên
            if (ModelState.IsValid)
            {
                // Kiểm tra: Tên khóa học đã được sử dụng bởi khóa học khác chưa?
                // Chúng ta chỉ cần kiểm tra tên có trùng với BẤT KỲ khóa học nào KHÁC ID hiện tại hay không.
                if (await _context.Courses.AnyAsync(c => c.Name == course.Name && c.Id != id))
                {
                    ModelState.AddModelError("Name", "Tên khóa học này đã được sử dụng cho một khóa học khác. Vui lòng chọn tên khác.");
                    return View(course);
                }

                try
                {
                    // Lấy Entity gốc (original entity) từ DB để cập nhật nếu cần,
                    // hoặc chỉ cần gọi Update nếu entity 'course' đã có đủ thông tin
                    // và không có vấn đề tracking nào khác.
                    _context.Update(course);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Course updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Courses.Any(e => e.Id == course.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(course);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);
            if (course == null)
            {
                return NotFound();
            }

            // Check if the course is assigned to any class
            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.CourseId == id);
            if (isAssigned)
            {
                ViewData["ErrorMessage"] = "This course cannot be deleted because it is assigned to one or more classes. Please remove the relevant assignments before deleting.";
            }

            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            // Re-check for safety before deleting
            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.CourseId == id);
            if (isAssigned)
            {
                TempData["ErrorMessage"] = "Deletion failed. The course is still assigned to a class.";
                // Redirect back to the delete confirmation page, which will now show the error
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The course has been deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error: Course not found for deletion.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
