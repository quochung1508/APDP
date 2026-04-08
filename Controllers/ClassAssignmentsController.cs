using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using SIMS.Models;
using SIMS.Models.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    public class ClassAssignmentsController : Controller
    {
        private readonly SimDbContext _context;

        public ClassAssignmentsController(SimDbContext context) { _context = context; }

        public async Task<IActionResult> Manage()
        {
            var allAssignments = await _context.ClassAssignments
                .Include(ca => ca.Class)
                .Include(ca => ca.Course)
                .Include(ca => ca.Teacher).ThenInclude(t => t.User)
                .Include(ca => ca.Student).ThenInclude(s => s.User)
                .Where(ca => ca.ClassId != null)
                .ToListAsync();

            var groupedByClass = allAssignments.GroupBy(ca => ca.Class);
            var viewModel = new ManageAssignmentsViewModel();

            foreach (var classGroup in groupedByClass)
            {
                var classViewModel = new ClassViewModel
                {
                    Id = classGroup.Key.Id,
                    Name = classGroup.Key.Name,
                    AssignmentGroups = classGroup.Where(ca => ca.CourseId != null).GroupBy(ca => ca.Course)
                        .Select(courseGroup => new AssignmentGroupViewModel
                        {
                            CourseId = courseGroup.Key.Id,
                            CourseName = courseGroup.Key.Name,
                            Teacher = courseGroup.FirstOrDefault(ca => ca.TeacherId != null)?.Teacher,
                            Students = courseGroup.Where(ca => ca.Student != null).Select(ca => ca.Student).Distinct().ToList()
                        }).ToList()
                };
                viewModel.Classes.Add(classViewModel);
            }

            var allClassIds = viewModel.Classes.Select(c => c.Id).ToList();
            var classesWithoutAssignments = await _context.Classes.Where(c => !allClassIds.Contains(c.Id)).ToListAsync();

            foreach (var emptyClass in classesWithoutAssignments)
            {
                viewModel.Classes.Add(new ClassViewModel { Id = emptyClass.Id, Name = emptyClass.Name, AssignmentGroups = new List<AssignmentGroupViewModel>() });
            }

            viewModel.Classes = viewModel.Classes.OrderBy(c => c.Name).ToList();
            return View(viewModel);
        }

        [HttpGet]
        public IActionResult CreateSchedule()
        {
            ViewBag.Classes = _context.Classes.ToList();
            ViewBag.Courses = _context.Courses.ToList();
            ViewBag.Teachers = _context.Teachers.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateSchedule(long ClassId, long CourseId, long TeacherId, string Schedule)
        {
            // 1. TẠO BẢN GHI GỐC: Đảm bảo lịch học không bao giờ bị mất
            bool hasMaster = await _context.ClassAssignments.AnyAsync(ca => ca.ClassId == ClassId && ca.CourseId == CourseId && ca.StudentId == null);
            if (!hasMaster)
            {
                _context.ClassAssignments.Add(new ClassAssignment
                {
                    ClassId = ClassId,
                    CourseId = CourseId,
                    TeacherId = TeacherId,
                    Schedule = Schedule,
                    StudentId = null
                });
            }

            // 2. GÁN MÔN CHO HỌC SINH
            var studentIdsInClass = await _context.ClassAssignments
                .Where(ca => ca.ClassId == ClassId && ca.StudentId != null && ca.CourseId == null).Select(ca => ca.StudentId.Value).Distinct().ToListAsync();

            foreach (var studentId in studentIdsInClass)
            {
                bool isEnrolled = await _context.ClassAssignments.AnyAsync(ca => ca.StudentId == studentId && ca.CourseId == CourseId && ca.ClassId == ClassId);
                if (!isEnrolled)
                {
                    _context.ClassAssignments.Add(new ClassAssignment
                    {
                        ClassId = ClassId,
                        CourseId = CourseId,
                        TeacherId = TeacherId,
                        Schedule = Schedule,
                        StudentId = studentId
                    });
                }
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đã lên thời khóa biểu và gán học sinh thành công!";
            return RedirectToAction("Manage");
        }

        [HttpGet]
        public async Task<IActionResult> GetStudentsForAssignment(long classId, long courseId)
        {
            var assignedStudentIds = await _context.ClassAssignments.Where(ca => ca.ClassId == classId && ca.CourseId == courseId && ca.StudentId.HasValue).Select(ca => ca.StudentId.Value).ToListAsync();
            var allStudentsInSystem = await _context.Students.Select(s => new { StudentId = s.Id, FullName = s.FullName }).ToListAsync();
            var assignedStudents = allStudentsInSystem.Where(s => assignedStudentIds.Contains(s.StudentId)).Select(s => new { id = s.StudentId, name = s.FullName }).ToList();
            var unassignedStudents = allStudentsInSystem.Where(s => !assignedStudentIds.Contains(s.StudentId)).Select(s => new { id = s.StudentId, name = s.FullName }).ToList();
            return Json(new { assigned = assignedStudents, unassigned = unassignedStudents });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssignments([FromBody] AssignmentUpdateViewModel model)
        {
            try
            {
                var assignmentsInDb = await _context.ClassAssignments.Where(ca => ca.ClassId == model.ClassId && ca.CourseId == model.CourseId).ToListAsync();
                var masterRecord = assignmentsInDb.FirstOrDefault(ca => ca.StudentId == null) ?? assignmentsInDb.FirstOrDefault();

                long? currentTeacherId = masterRecord?.TeacherId;
                string currentSchedule = masterRecord?.Schedule;

                if (model.StudentIdsToRemove != null && model.StudentIdsToRemove.Any())
                {
                    var toRemove = assignmentsInDb.Where(ca => ca.StudentId.HasValue && model.StudentIdsToRemove.Contains(ca.StudentId.Value)).ToList();
                    if (toRemove.Any()) _context.ClassAssignments.RemoveRange(toRemove);
                }

                if (model.StudentIdsToAdd != null && model.StudentIdsToAdd.Any())
                {
                    var assignmentsToAdd = new List<ClassAssignment>();
                    foreach (var studentId in model.StudentIdsToAdd)
                    {
                        if (!assignmentsInDb.Any(ca => ca.StudentId == studentId))
                        {
                            assignmentsToAdd.Add(new ClassAssignment
                            {
                                ClassId = model.ClassId,
                                CourseId = model.CourseId,
                                StudentId = studentId,
                                TeacherId = currentTeacherId,
                                Schedule = currentSchedule
                            });
                        }
                    }
                    if (assignmentsToAdd.Any()) await _context.ClassAssignments.AddRangeAsync(assignmentsToAdd);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Cập nhật sinh viên thành công!" });
            }
            catch (System.Exception ex) { return Json(new { success = false, message = "Có lỗi xảy ra: " + ex.Message }); }
        }
    }
}