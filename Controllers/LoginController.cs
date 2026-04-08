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
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // 1. Tìm user bằng Username (thay vì Email)
                // Nếu file LoginViewModel của bạn viết là UserName (chữ N hoa) thì đổi lại thành model.UserName nhé
                var user = await _userManager.FindByNameAsync(model.Username);

                if (user != null)
                {
                    // 2. Thay model.RememberMe bằng false (vì bạn không có chức năng này)
                    var result = await _signInManager.PasswordSignInAsync(user.UserName, model.Password, isPersistent: false, lockoutOnFailure: false);

                    if (result.Succeeded)
                    {
                        // -- KIỂM TRA MẬT KHẨU MẶC ĐỊNH (ÉP ĐỔI LẦN ĐẦU) --
                        var claims = await _userManager.GetClaimsAsync(user);
                        if (claims.Any(c => c.Type == "MustChangePassword" && c.Value == "true"))
                        {
                            return RedirectToAction("ChangePassword", "Auth");
                        }

                        return RedirectToAction("Index", "Dashboard");
                    }
                }

                // 3. THÔNG BÁO LỖI CHUNG CHUNG
                ModelState.AddModelError(string.Empty, "Tên đăng nhập hoặc mật khẩu không chính xác.");
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
