using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.Models;
using SIMS.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SIMS.Controllers
{
    public class ClassAssignmentsController : Controller
    {
        private readonly SimDbContext _context;

        public ClassAssignmentsController(SimDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Manage()
        {
            // **LOGIC TO DISPLAY OVERVIEW**
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
                    AssignmentGroups = classGroup
                        .Where(ca => ca.CourseId != null)
                        .GroupBy(ca => ca.Course)
                        .Select(courseGroup => new AssignmentGroupViewModel
                        {
                            CourseId = courseGroup.Key.Id,
                            CourseName = courseGroup.Key.Name,
                            Teacher = courseGroup.FirstOrDefault()?.Teacher,
                            Students = courseGroup.Where(ca => ca.Student != null).Select(ca => ca.Student).Distinct().ToList()
                        }).ToList()
                };
                viewModel.Classes.Add(classViewModel);
            }

            var allClassIds = viewModel.Classes.Select(c => c.Id).ToList();
            var classesWithoutAssignments = await _context.Classes
                .Where(c => !allClassIds.Contains(c.Id))
                .ToListAsync();

            foreach (var emptyClass in classesWithoutAssignments)
            {
                viewModel.Classes.Add(new ClassViewModel
                {
                    Id = emptyClass.Id,
                    Name = emptyClass.Name,
                    AssignmentGroups = new System.Collections.Generic.List<AssignmentGroupViewModel>()
                });
            }

            viewModel.Classes = viewModel.Classes.OrderBy(c => c.Name).ToList();

            // **PREPARE DATA FOR THE EDIT MODAL**
            var teachers = await _context.Teachers.Include(t => t.User).ToListAsync();
            ViewData["Teachers"] = new SelectList(teachers.Select(t => new { Id = t.Id, Name = t.FullName }), "Id", "Name");
            ViewData["Courses"] = new SelectList(_context.Courses, "Id", "Name");
            ViewData["Classes"] = new SelectList(_context.Classes, "Id", "Name");

            return View(viewModel);
        }

        // **API ENDPOINT**
        [HttpGet]
        public async Task<IActionResult> GetStudentsForAssignment(long classId, long courseId)
        {
            var assignedStudentIds = await _context.ClassAssignments
                .Where(ca => ca.ClassId == classId && ca.CourseId == courseId && ca.StudentId.HasValue)
                .Select(ca => ca.StudentId.Value)
                .ToListAsync();

            var allStudentsInSystem = await _context.Students
                .Include(s => s.User)
                .Select(s => new {
                    StudentId = s.Id,
                    UserName = s.User.UserName,
                    FullName = s.FullName
                })
                .ToListAsync();

            var assignedStudents = allStudentsInSystem
                .Where(s => assignedStudentIds.Contains(s.StudentId))
                .Select(s => new { id = s.StudentId, name = s.FullName })
                .ToList();

            var unassignedStudents = allStudentsInSystem
                .Where(s => !assignedStudentIds.Contains(s.StudentId))
                .Select(s => new { id = s.StudentId, name = s.FullName })
                .ToList();

            var teacherAssignment = await _context.ClassAssignments
                .Include(ca => ca.Teacher)
                .FirstOrDefaultAsync(ca => ca.ClassId == classId && ca.CourseId == courseId && ca.TeacherId != null);

            long? teacherId = teacherAssignment?.TeacherId;

            return Json(new { assigned = assignedStudents, unassigned = unassignedStudents, teacherId = teacherId });
        }

        // **API ENDPOINT**
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAssignments([FromBody] AssignmentUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid data submitted." });
            }

            try
            {
                var assignmentsInDb = await _context.ClassAssignments
                   .Where(ca => ca.ClassId == model.ClassId && ca.CourseId == model.CourseId)
                   .ToListAsync();

                var teacherAssignmentInDb = assignmentsInDb.FirstOrDefault(ca => ca.TeacherId != null);

                // 1. Handle Teacher Assignment
                if (model.TeacherId > 0)
                {
                    if (teacherAssignmentInDb == null) // No teacher assigned, create new
                    {
                        _context.ClassAssignments.Add(new DatabaseContext.Entities.ClassAssignment
                        {
                            ClassId = model.ClassId,
                            CourseId = model.CourseId,
                            TeacherId = model.TeacherId
                        });
                    }
                    else if (teacherAssignmentInDb.TeacherId != model.TeacherId) // Exists and has been changed
                    {
                        teacherAssignmentInDb.TeacherId = model.TeacherId;
                        _context.ClassAssignments.Update(teacherAssignmentInDb);
                    }
                }
                else // TeacherId = 0, means unassigning the teacher
                {
                    if (teacherAssignmentInDb != null)
                    {
                        _context.ClassAssignments.Remove(teacherAssignmentInDb);
                    }
                }

                // 2. Handle Student Removals
                if (model.StudentIdsToRemove != null && model.StudentIdsToRemove.Any())
                {
                    var studentAssignmentsToRemove = assignmentsInDb
                        .Where(ca => ca.StudentId.HasValue && model.StudentIdsToRemove.Contains(ca.StudentId.Value))
                        .ToList();
                    if (studentAssignmentsToRemove.Any())
                    {
                        _context.ClassAssignments.RemoveRange(studentAssignmentsToRemove);
                    }
                }

                // 3. Handle Student Additions
                if (model.StudentIdsToAdd != null && model.StudentIdsToAdd.Any())
                {
                    if (model.TeacherId <= 0)
                    {
                        return Json(new { success = false, message = "A teacher must be assigned to the course before adding students." });
                    }

                    var assignmentsToAdd = new List<DatabaseContext.Entities.ClassAssignment>();
                    foreach (var studentId in model.StudentIdsToAdd)
                    {
                        if (!assignmentsInDb.Any(ca => ca.StudentId == studentId))
                        {
                            assignmentsToAdd.Add(new DatabaseContext.Entities.ClassAssignment
                            {
                                ClassId = model.ClassId,
                                CourseId = model.CourseId,
                                StudentId = studentId,
                                TeacherId = model.TeacherId // Assign the current teacher to the student
                            });
                        }
                    }
                    if (assignmentsToAdd.Any())
                    {
                        await _context.ClassAssignments.AddRangeAsync(assignmentsToAdd);
                    }
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Assignments updated successfully!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred.", error = ex.Message });
            }
        }
    }
}
