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

            // 3. Đăng ký Services tùy chỉnh
            builder.Services.AddScoped<IAccountService, AccountService>();

            // 4. Thêm MVC
            builder.Services.AddControllersWithViews();

            // 5. Cấu hình Cookie
            builder.Services.AddHttpContextAccessor();
            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Login";
                options.AccessDeniedPath = "/Auth/AccessDenied";
            });

            // 6. Cấu hình Authorization
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
                options.AddPolicy("StudentOnly", policy => policy.RequireRole("Student"));
                options.AddPolicy("FacultyOnly", policy => policy.RequireRole("Faculty"));
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
