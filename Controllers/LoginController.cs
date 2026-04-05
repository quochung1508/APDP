using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIMS.DatabaseContext.Entities;
using SIMS.Models;

namespace SIMS.Controllers
{
    public class LoginController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole<long>> _roleManager;

        public LoginController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole<long>> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Username, model.Password, false, false);
                if (result.Succeeded)
                {
                    // Get the user who just logged in
                    var user = await _userManager.FindByNameAsync(model.Username);
                    if (user != null)
                    {
                        var roles = await _userManager.GetRolesAsync(user);
                        var role = roles.FirstOrDefault(); // Get the primary role

                        // Redirect based on role
                        if (role == "Admin")
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (role == "Student")
                        {
                            return RedirectToAction("Schedule", "Student");
                        }
                        else if (role == "Teacher")
                        {
                            return RedirectToAction("Schedule", "Teacher");
                        }
                    }
                    // If there is no specific role, redirect to the default Home page
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }

        // ==================================================================
        // TEMPORARY FUNCTION TO CREATE ADMIN - CAN BE DELETED AFTER USE
        // ==================================================================
        [HttpGet]
        public async Task<IActionResult> SetupAdmin()
        {
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole<long>("Admin"));
            }

            var adminUser = await _userManager.FindByNameAsync("admin");
            if (adminUser == null)
            {
                adminUser = new ApplicationUser { UserName = "admin", Email = "admin@example.com", EmailConfirmed = true };
                var createUserResult = await _userManager.CreateAsync(adminUser, "Admin@123");

                if (createUserResult.Succeeded)
                {
                    await _userManager.AddToRoleAsync(adminUser, "Admin");
                    return Content("Successfully created user 'admin' with role 'Admin'. Please return to the login page.");
                }
                else
                {
                    return Content("Error creating user: " + string.Join(", ", createUserResult.Errors.Select(e => e.Description)));
                }
            }

            if (!await _userManager.IsInRoleAsync(adminUser, "Admin"))
            {
                await _userManager.AddToRoleAsync(adminUser, "Admin");
            }

            return Content("User 'admin' already exists and has been assigned the 'Admin' role.");
        }
    }
}
