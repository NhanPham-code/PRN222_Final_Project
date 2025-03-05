using BLL.Interfaces;
using BLL.Services;
using DataAccess.DAOs;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

namespace PRN222_Final_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Inject DB
            builder.Services.AddDbContext<BakeryShopDbContext>(options =>
                           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Inject Dao
            builder.Services.AddScoped(typeof(ICrudDAO<,>), typeof(CrudDAO<,>));

            // Inject Repo
            builder.Services.AddScoped(typeof(ICrudRepo<,>), typeof(CrudRepo<,>));

            // Inject Product Service
            builder.Services.AddScoped(typeof(ProductService));

            // Inject Email Service
            builder.Services.AddScoped(typeof(EmailService));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại trong 30 phút
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            /*builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(options =>
            {
                options.LoginPath = "/Common/Login"; // Đường dẫn đến trang đăng nhập
                options.LogoutPath = "/Common/Logout"; // Đường dẫn đăng xuất
                options.AccessDeniedPath = "/Common/AccessDenied"; // Trang lỗi nếu không có quyền
            });*/


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession(); // add session

            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
