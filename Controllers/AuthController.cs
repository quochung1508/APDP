using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SIMS.DatabaseContext.Entities;
using SIMS.Models.ViewModels;

namespace SIMS.Controllers
{    
    [Authorize] // Bắt buộc phải đăng nhập mới được đổi mật khẩu
    public class AuthController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction("Index", "Login");

            // Đổi mật khẩu sử dụng hàm có sẵn của Identity (không cần đụng DB)
            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);

            if (result.Succeeded)
            {
                // THÊM ĐOẠN NÀY: Tìm và gỡ bỏ cờ ép đổi mật khẩu để lần sau đăng nhập bình thường
                var claims = await _userManager.GetClaimsAsync(user);
                var mustChangeClaim = claims.FirstOrDefault(c => c.Type == "MustChangePassword");
                if (mustChangeClaim != null)
                {
                    await _userManager.RemoveClaimAsync(user, mustChangeClaim);
                }

                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
                return RedirectToAction("Index", "Dashboard");
            }

            // Xử lý lỗi (vd: Mật khẩu cũ không đúng, mật khẩu mới quá ngắn...)
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View(model);
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
