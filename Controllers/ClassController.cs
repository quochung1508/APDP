using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using SIMS.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    public class ClassController : Controller
    {
        private readonly SimDbContext _context;

        public ClassController(SimDbContext context)
        {
            _context = context;
        }

        // GET: Class
        public async Task<IActionResult> Index(string searchString)
        {
            var classesQuery = _context.Classes.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                classesQuery = classesQuery.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Schedule.Contains(searchString)
                );
            }

            var classes = await classesQuery.OrderBy(c => c.Name).ToListAsync();

            ViewData["CurrentFilter"] = searchString;

            return View(classes);
        }


        // GET: Class/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);
            if (@class == null)
            {
                return NotFound();
            }

            var assignments = await _context.ClassAssignments
                .Where(ca => ca.ClassId == id)
                .Include(ca => ca.Student)
                .Include(ca => ca.Course)
                .OrderBy(ca => ca.Course.Name)
                .ThenBy(ca => ca.Student.FullName)
                .ToListAsync();

            var viewModel = new ClassDetailsViewModel
            {
                ClassId = @class.Id,
                ClassName = @class.Name,
                Schedule = @class.Schedule // Fixed: Added Schedule assignment
            };

            var groupedByCourse = assignments.GroupBy(a => a.Course);

            foreach (var group in groupedByCourse)
            {
                if (group.Key != null) // Ensure the course is not null
                {
                    var studentsInfo = group.Select(a => a.Student).Where(s => s != null).Select(s => new ClassDetailsViewModel.StudentInfo
                    {
                        StudentId = s.StudentNumber,
                        FullName = s.FullName,
                        PhoneNumber = s.PhoneNumber
                    }).ToList();

                    viewModel.CourseRoster[group.Key.Name] = studentsInfo;
                }
            }

            return View(viewModel);
        }


        // GET: Class/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Class/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Schedule")] Class @class)
        {
            if (ModelState.IsValid)
            {
                // **Bổ sung: Kiểm tra xem tên lớp đã tồn tại chưa**
                var existingClass = await _context.Classes.FirstOrDefaultAsync(c => c.Name == @class.Name);
                if (existingClass != null)
                {
                    // Tên lớp đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Name", "This class name already exists. Please choose another name.");
                    return View(@class);
                }

                _context.Add(@class);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Class created successfully.";
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        // GET: Class/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }


            var @class = await _context.Classes.FindAsync(id);
            if (@class == null)
            {
                return NotFound();
            }

            var @newclass = await _context.Classes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id);
            if (@class == newclass)
            {
                return NotFound();
            }

            return View(@class);

        }

        // POST: Class/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Schedule")] Class @class)
        {
            if (id != @class.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Kiểm tra: Tên lớp đã tồn tại với ID khác không?
                var existingClass = await _context.Classes
                    .AsNoTracking() // Dùng AsNoTracking để tránh lỗi theo dõi
                    .FirstOrDefaultAsync(c => c.Name == @class.Name && c.Id != id); // Kiểm tra ID khác

                if (existingClass != null)
                {
                    // Tên lớp đã tồn tại, thêm lỗi vào ModelState
                    ModelState.AddModelError("Name", "This class name is already in use for another class. Please choose another name.");
                    return View(@class);
                }

                try
                {
                    _context.Update(@class);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Class updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Classes.Any(e => e.Id == @class.Id))
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
            return View(@class);
        }


        // GET: Class/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @class = await _context.Classes
                .FirstOrDefaultAsync(m => m.Id == id);

            if (@class == null)
            {
                return NotFound();
            }

            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.ClassId == id);

            if (isAssigned)
            {
                ViewData["ErrorMessage"] = "This class cannot be deleted because it is assigned to one or more students, teachers, or courses. Please remove all assignments before deleting.";
            }

            return View(@class);
        }

        // POST: Class/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.ClassId == id);
            if (isAssigned)
            {
                TempData["ErrorMessage"] = "Deletion failed. The class is still assigned. Please ensure all assignments are removed first.";
                return RedirectToAction(nameof(Delete), new { id = id });
            }

            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "The class has been deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Error: Class not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
