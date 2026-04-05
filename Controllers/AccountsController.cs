using Microsoft.AspNetCore.Mvc;
using SIMS.Models;
using Microsoft.AspNetCore.Identity;
using SIMS.DatabaseContext.Entities;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using System;
using System.Collections.Generic;

namespace SIMS.Controllers
{
    public class AccountsController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;
        private readonly SimDbContext _context;

        public AccountsController(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<long>> roleManager,
            SimDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        #region Create Actions

        [HttpGet]
        public IActionResult CreateStudent()
        {
            var model = new CreateAccountViewModel { Role = "Student" };
            return View("Create", model);
        }

        [HttpGet]
        public IActionResult CreateTeacher()
        {
            var model = new CreateAccountViewModel { Role = "Teacher" };
            return View("Create", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Add role to user
                if (!string.IsNullOrWhiteSpace(model.Role))
                {
                    if (!await _roleManager.RoleExistsAsync(model.Role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole<long>(model.Role));
                    }
                    await _userManager.AddToRoleAsync(user, model.Role);
                }

                // Only create a profile if personal information (like FullName) is provided.
                if (!string.IsNullOrWhiteSpace(model.FullName))
                {
                    if (model.Role == "Student")
                    {
                        var student = new Student
                        {
                            UserId = user.Id,
                            StudentNumber = model.ProfileId,
                            FullName = model.FullName,
                            DateOfBirth = model.DateOfBirth,
                            Gender = model.Gender,
                            Address = model.Address,
                            PhoneNumber = model.PhoneNumber
                        };
                        _context.Students.Add(student);
                    }
                    else if (model.Role == "Teacher")
                    {
                        var teacher = new Teacher
                        {
                            UserId = user.Id,
                            TeacherNumber = model.ProfileId,
                            FullName = model.FullName,
                            DateOfBirth = model.DateOfBirth,
                            Gender = model.Gender,
                            Address = model.Address,
                            PhoneNumber = model.PhoneNumber
                        };
                        _context.Teachers.Add(teacher);
                    }

                    // Save the profile changes
                    await _context.SaveChangesAsync();
                }

                TempData["SuccessMessage"] = $"Account {user.UserName} has been created.";
                return RedirectToAction("ManageUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }

        #endregion

        #region Edit Actions

        [HttpGet]
        public async Task<IActionResult> Edit(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            var model = new EditUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
            };

            if (role == "Student")
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (student != null)
                {
                    model.ProfileId = student.StudentNumber;
                    model.FullName = student.FullName;
                    model.DateOfBirth = student.DateOfBirth;
                    model.Gender = student.Gender;
                    model.Address = student.Address;
                    model.PhoneNumber = student.PhoneNumber;
                }
            }
            else if (role == "Teacher")
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher != null)
                {
                    model.ProfileId = teacher.TeacherNumber;
                    model.FullName = teacher.FullName;
                    model.DateOfBirth = teacher.DateOfBirth;
                    model.Gender = teacher.Gender;
                    model.Address = teacher.Address;
                    model.PhoneNumber = teacher.PhoneNumber;
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id.ToString());
            if (user == null) return NotFound();

            // Update user info
            user.Email = model.Email;
            user.UserName = model.UserName;
            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
            {
                foreach (var error in updateResult.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
                return View(model);
            }

            // --- SMART PROFILE UPDATE LOGIC ---
            if (model.Role == "Student")
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (student != null) // If profile exists, update it.
                {
                    student.StudentNumber = model.ProfileId;
                    student.FullName = model.FullName;
                    student.DateOfBirth = model.DateOfBirth;
                    student.Gender = model.Gender;
                    student.Address = model.Address;
                    student.PhoneNumber = model.PhoneNumber;
                    _context.Students.Update(student);
                }
                else // If profile does not exist, create a new one.
                {
                    var newStudent = new Student
                    {
                        UserId = user.Id,
                        StudentNumber = model.ProfileId,
                        FullName = model.FullName,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber
                    };
                    _context.Students.Add(newStudent);
                }
            }
            else if (model.Role == "Teacher")
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher != null) // If profile exists, update it.
                {
                    teacher.TeacherNumber = model.ProfileId;
                    teacher.FullName = model.FullName;
                    teacher.DateOfBirth = model.DateOfBirth;
                    teacher.Gender = model.Gender;
                    teacher.Address = model.Address;
                    teacher.PhoneNumber = model.PhoneNumber;
                    _context.Teachers.Update(teacher);
                }
                else // If profile does not exist, create a new one.
                {
                    var newTeacher = new Teacher
                    {
                        UserId = user.Id,
                        TeacherNumber = model.ProfileId,
                        FullName = model.FullName,
                        DateOfBirth = model.DateOfBirth,
                        Gender = model.Gender,
                        Address = model.Address,
                        PhoneNumber = model.PhoneNumber
                    };
                    _context.Teachers.Add(newTeacher);
                }
            }

            await _context.SaveChangesAsync();

            // Handle password change
            if (!string.IsNullOrEmpty(model.OldPassword) && !string.IsNullOrEmpty(model.NewPassword))
            {
                var changePasswordResult = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (!changePasswordResult.Succeeded)
                {
                    foreach (var error in changePasswordResult.Errors) { ModelState.AddModelError(string.Empty, error.Description); }
                    return View(model);
                }
                TempData["SuccessMessage"] = "User information and password updated successfully.";
            }
            else
            {
                TempData["SuccessMessage"] = "User information updated successfully.";
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        #endregion

        #region Delete Actions

        [HttpGet]
        public async Task<IActionResult> Delete(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault();

            string warningMessage = null;
            bool showWarning = false;

            if (role == "Student")
            {
                var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                if (student != null)
                {
                    var isEnrolled = await _context.ClassAssignments.AnyAsync(ca => ca.StudentId == student.Id);
                    if (isEnrolled)
                    {
                        showWarning = true;
                        warningMessage = "This student is currently enrolled in one or more classes. Deleting this user will also remove them from all classes. Do you want to proceed?";
                    }
                }
            }
            else if (role == "Teacher")
            {
                var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                if (teacher != null)
                {
                    var isAssigned = await _context.ClassAssignments.AnyAsync(ca => ca.TeacherId == teacher.Id);
                    if (isAssigned)
                    {
                        showWarning = true;
                        warningMessage = "This teacher is currently assigned to one or more classes. Deleting this user will remove their assignments. Do you want to proceed?";
                    }
                }
            }

            ViewData["ShowClassWarning"] = showWarning;
            ViewData["WarningText"] = warningMessage;

            var model = new DeleteUserViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Role = role
            };

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction(nameof(ManageUsers));
            }

            using (var transaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    var roles = await _userManager.GetRolesAsync(user);
                    var role = roles.FirstOrDefault();

                    if (role == "Student")
                    {
                        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
                        if (student != null)
                        {
                            // Remove attendance records
                            var attendances = _context.Attendances.Where(a => a.StudentId == student.Id);
                            if (attendances.Any()) _context.Attendances.RemoveRange(attendances);

                            // Remove class assignments
                            var assignments = _context.ClassAssignments.Where(ca => ca.StudentId == student.Id);
                            if (assignments.Any()) _context.ClassAssignments.RemoveRange(assignments);

                            _context.Students.Remove(student);
                        }
                    }
                    else if (role == "Teacher")
                    {
                        var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == user.Id);
                        if (teacher != null)
                        {
                            // Remove attendance records
                            var attendances = _context.Attendances.Where(a => a.TeacherId == teacher.Id);
                            if (attendances.Any()) _context.Attendances.RemoveRange(attendances);

                            // Remove class assignments
                            var assignments = _context.ClassAssignments.Where(ca => ca.TeacherId == teacher.Id);
                            if (assignments.Any()) _context.ClassAssignments.RemoveRange(assignments);

                            _context.Teachers.Remove(teacher);
                        }
                    }

                    await _context.SaveChangesAsync();

                    if (roles.Any())
                    {
                        var removeRolesResult = await _userManager.RemoveFromRolesAsync(user, roles);
                        if (!removeRolesResult.Succeeded)
                        {
                            await transaction.RollbackAsync();
                            var errors = string.Join(", ", removeRolesResult.Errors.Select(e => e.Description));
                            TempData["ErrorMessage"] = $"Failed to remove user roles: {errors}";
                            return RedirectToAction(nameof(ManageUsers));
                        }
                    }

                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        await transaction.CommitAsync();
                        TempData["SuccessMessage"] = $"User {user.UserName} and all related data have been deleted successfully.";
                    }
                    else
                    {
                        await transaction.RollbackAsync();
                        var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                        TempData["ErrorMessage"] = $"Failed to delete user: {errors}";
                    }
                }
                catch (DbUpdateException ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"An error occurred while saving to the database: {ex.InnerException?.Message ?? ex.Message}";
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    TempData["ErrorMessage"] = $"An unexpected error occurred: {ex.Message}";
                }
            }

            return RedirectToAction(nameof(ManageUsers));
        }

        #endregion

        #region Manage Users & Teachers

        public async Task<IActionResult> ManageUsers(string role = "All", string searchString = "")
        {
            var allUsers = await _userManager.Users.ToListAsync();
            var allStudents = await _context.Students.ToDictionaryAsync(s => s.UserId, s => s.PhoneNumber);
            var allTeachers = await _context.Teachers.ToDictionaryAsync(t => t.UserId, t => t.PhoneNumber);

            var model = new List<UserListItemViewModel>();

            foreach (var u in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(u);
                var primaryRole = roles.FirstOrDefault() ?? string.Empty;

                if (!string.Equals(role, "All", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(primaryRole, role, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                string phoneNumber = string.Empty;
                if (primaryRole == "Student" && allStudents.ContainsKey(u.Id))
                {
                    phoneNumber = allStudents[u.Id];
                }
                else if (primaryRole == "Teacher" && allTeachers.ContainsKey(u.Id))
                {
                    phoneNumber = allTeachers[u.Id];
                }


                if (!string.IsNullOrEmpty(searchString))
                {
                    bool matchesSearch = (u.UserName != null && u.UserName.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                         (u.Email != null && u.Email.Contains(searchString, StringComparison.OrdinalIgnoreCase)) ||
                                         u.Id.ToString().Contains(searchString) ||
                                         (!string.IsNullOrEmpty(phoneNumber) && phoneNumber.Contains(searchString));

                    if (!matchesSearch)
                    {
                        continue;
                    }
                }

                model.Add(new UserListItemViewModel
                {
                    Id = u.Id,
                    UserName = u.UserName ?? string.Empty,
                    Email = u.Email ?? string.Empty,
                    Role = primaryRole,
                    PhoneNumber = phoneNumber ?? "N/A"
                });
            }

            ViewData["ActiveRoleFilter"] = role ?? "All";
            ViewData["CurrentFilter"] = searchString;
            ViewData["UsersCount"] = model.Count;
            return View(model.OrderBy(u => u.UserName).ToList());
        }


        public async Task<IActionResult> ManageTeachers(string searchString = "")
        {
            return await ManageUsers("Teacher", searchString);
        }

        #endregion
    }
}
