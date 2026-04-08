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

        public async Task<IActionResult> Index(string searchString)
        {
            var classesQuery = _context.Classes.AsQueryable();

            if (!string.IsNullOrEmpty(searchString))
            {
                classesQuery = classesQuery.Where(c =>
                    c.Name.Contains(searchString) ||
                    c.Cohort.Contains(searchString)
                );
            }

            var classes = await classesQuery.OrderBy(c => c.Name).ToListAsync();
            ViewData["CurrentFilter"] = searchString;

            return View(classes);
        }

        public async Task<IActionResult> Details(long? id)
        {
            if (id == null) return NotFound();

            var @class = await _context.Classes.FirstOrDefaultAsync(c => c.Id == id);
            if (@class == null) return NotFound();

            var assignments = await _context.ClassAssignments
                .Where(ca => ca.ClassId == id)
                .Include(ca => ca.Student)
                .Include(ca => ca.Course)
                .OrderBy(ca => ca.Course.Name)
                .ThenBy(ca => ca.Student.FullName)
                .ToListAsync();

            var userIds = assignments.Where(a => a.Student != null).Select(a => a.Student.UserId).Distinct().ToList();
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.Email);

            var viewModel = new ClassDetailsViewModel
            {
                ClassId = @class.Id,
                ClassName = @class.Name,
                Cohort = @class.Cohort // ĐÃ SỬA LỖI Ở ĐÂY: Dùng Cohort thay vì Schedule
            };

            var allEmailsList = new List<string>();
            var groupedByCourse = assignments.GroupBy(a => a.Course);

            foreach (var group in groupedByCourse)
            {
                if (group.Key != null)
                {
                    var studentsInfo = new List<ClassDetailsViewModel.StudentInfo>();

                    foreach (var a in group)
                    {
                        if (a.Student != null)
                        {
                            string email = users.ContainsKey(a.Student.UserId) ? users[a.Student.UserId] : "N/A";
                            if (email != "N/A" && !string.IsNullOrEmpty(email)) allEmailsList.Add(email);

                            studentsInfo.Add(new ClassDetailsViewModel.StudentInfo
                            {
                                StudentId = a.Student.StudentNumber,
                                FullName = a.Student.FullName,
                                PhoneNumber = a.Student.PhoneNumber,
                                Email = email
                            });
                        }
                    }
                    viewModel.CourseRoster[group.Key.Name] = studentsInfo;
                }
            }

            viewModel.AllEmails = string.Join(",", allEmailsList.Distinct());
            return View(viewModel);
        }

        public async Task<IActionResult> ExportCsv(long id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();

            var assignments = await _context.ClassAssignments
                .Where(ca => ca.ClassId == id)
                .Include(ca => ca.Student)
                .Include(ca => ca.Course)
                .OrderBy(ca => ca.Course.Name)
                .ThenBy(ca => ca.Student.FullName)
                .ToListAsync();

            var userIds = assignments.Where(a => a.Student != null).Select(a => a.Student.UserId).Distinct().ToList();
            var users = await _context.Users.Where(u => userIds.Contains(u.Id)).ToDictionaryAsync(u => u.Id, u => u.Email);

            var builder = new System.Text.StringBuilder();
            builder.Append('\uFEFF');
            builder.AppendLine("Course,Student ID,Full Name,Phone Number,Email");

            foreach (var a in assignments.Where(a => a.Student != null && a.Course != null))
            {
                string email = users.ContainsKey(a.Student.UserId) ? users[a.Student.UserId] : "";
                builder.AppendLine($"{a.Course.Name},{a.Student.StudentNumber},{a.Student.FullName},{a.Student.PhoneNumber},{email}");
            }

            return File(System.Text.Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", $"DanhSachLop_{@class.Name}.csv");
        }

        public IActionResult Create()
        {
            ViewBag.Students = _context.Students
                .Select(s => new {
                    Id = s.Id,
                    DisplayText = s.StudentNumber + " - " + s.FullName
                }).ToList();

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(string Name, string Cohort, List<long> SelectedStudentIds)
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                ModelState.AddModelError("Name", "Tên lớp không được để trống.");
                ViewBag.Students = _context.Students.Select(s => new { Id = s.Id, DisplayText = s.StudentNumber + " - " + s.FullName }).ToList();
                return View();
            }

            var existingClass = await _context.Classes.FirstOrDefaultAsync(c => c.Name == Name);
            if (existingClass != null)
            {
                ModelState.AddModelError("Name", "Tên lớp này đã tồn tại. Vui lòng chọn tên khác.");
                ViewBag.Students = _context.Students.Select(s => new { Id = s.Id, DisplayText = s.StudentNumber + " - " + s.FullName }).ToList();
                return View();
            }

            var newClass = new Class
            {
                Name = Name,
                Cohort = string.IsNullOrWhiteSpace(Cohort) ? "Chưa có khóa học" : Cohort
            };

            _context.Classes.Add(newClass);
            await _context.SaveChangesAsync();

            if (SelectedStudentIds != null && SelectedStudentIds.Any())
            {
                foreach (var studentId in SelectedStudentIds)
                {
                    _context.ClassAssignments.Add(new ClassAssignment
                    {
                        ClassId = newClass.Id,
                        StudentId = studentId
                    });
                }
                await _context.SaveChangesAsync();
            }

            TempData["SuccessMessage"] = "Tạo lớp và thêm danh sách học sinh thành công!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes.FindAsync(id);
            if (@class == null) return NotFound();
            return View(@class);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,Name,Cohort")] Class @class) // ĐÃ SỬA BIND
        {
            if (id != @class.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var existingClass = await _context.Classes
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Name == @class.Name && c.Id != id);

                if (existingClass != null)
                {
                    ModelState.AddModelError("Name", "This class name is already in use.");
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
                    if (!_context.Classes.Any(e => e.Id == @class.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@class);
        }

        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null) return NotFound();
            var @class = await _context.Classes.FirstOrDefaultAsync(m => m.Id == id);
            if (@class == null) return NotFound();

            // Thay đổi cảnh báo: Báo cho Admin biết sẽ xóa cả dữ liệu liên đới
            var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.ClassId == id);
            if (isAssigned)
            {
                ViewData["WarningMessage"] = "Lớp này đang có sinh viên và thời khóa biểu. Xóa lớp này sẽ đồng thời xóa toàn bộ danh sách và lịch học liên quan.";
            }
            return View(@class);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var @class = await _context.Classes.FindAsync(id);
            if (@class != null)
            {
                // 1. Xóa toàn bộ lịch sử điểm danh (nếu có) của lớp này
                var attendances = _context.Attendances.Where(a => a.ClassId == id);
                if (attendances.Any()) _context.Attendances.RemoveRange(attendances);

                // 2. Xóa toàn bộ danh sách học sinh và thời khóa biểu của lớp này
                var assignments = _context.ClassAssignments.Where(ca => ca.ClassId == id);
                if (assignments.Any()) _context.ClassAssignments.RemoveRange(assignments);

                // 3. Tiến hành xóa Lớp
                _context.Classes.Remove(@class);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã xóa lớp học và dọn dẹp toàn bộ dữ liệu liên quan thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy lớp học cần xóa.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}