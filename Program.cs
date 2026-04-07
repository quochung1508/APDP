// File: Program.cs (Đã sửa lại tên DbContext cho chính xác)

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SIMS.DatabaseContext;
using SIMS.DatabaseContext.Entities;
using SIMS.Interfaces;
using SIMS.Services;

namespace SIMS
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ==========================================================
            // Cấu hình DỊCH VỤ (SERVICES)
            // ==========================================================

            // 1. Cấu hình Database - Sử dụng tên chính xác: SimDbContext
            builder.Services.AddDbContext<SimDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            // 2. Cấu hình Identity - Sử dụng tên chính xác: SimDbContext
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<long>>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 1;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequiredUniqueChars = 0;
                options.User.RequireUniqueEmail = false;
            })
                .AddEntityFrameworkStores<SimDbContext>() // Đã sửa lại
                .AddDefaultTokenProviders()
                .AddRoleManager<RoleManager<IdentityRole<long>>>();
            
            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Báo cho hệ thống biết trang Login nằm ở đâu
                options.LoginPath = "/Login/Index";

                // Báo cho hệ thống biết trang Từ chối truy cập nằm ở đâu
                // (Dựa vào file Views/Auth/AccessDenied.cshtml của bạn)
                options.AccessDeniedPath = "/Auth/AccessDenied";
            });

            // 3. Đăng ký Services tùy chỉnh
            builder.Services.AddScoped<IAccountService, AccountService>();

            // 4. Thêm MVC
            builder.Services.AddControllersWithViews();

            // 5. Cấu hình Cookie (Đã xoá phần khai báo trùng lặp)
            builder.Services.AddHttpContextAccessor();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login/Index";

                // MẸO: Trỏ tạm về trang Home nếu bạn chưa tạo trang AccessDenied 
                // để tránh lỗi 404 nếu sau này có lỡ truy cập sai quyền
                options.AccessDeniedPath = "/Home/Index";
            });

            // 6. Cấu hình Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));

                // ĐÃ SỬA: Đổi FacultyOnly thành TeacherOnly và dùng role "Teacher"
                options.AddPolicy("TeacherOnly", policy => policy.RequireRole("Teacher"));
            });

            // ==========================================================
            // Xây dựng và Cấu hình PIPELINE
            // ==========================================================

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // Added this line
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Login}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
