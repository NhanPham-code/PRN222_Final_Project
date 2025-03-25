using BLL.Interfaces;
using BLL.Services;
using DataAccess.DAOs;
using DataAccess.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using PRN222_Final_Project.SignalRHub;

namespace PRN222_Final_Project
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            // Inject SignalR
            builder.Services.AddSignalR();

            // Inject DB
            builder.Services.AddDbContext<BakeryShopDbContext>(options =>
                           options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Inject Dao
            builder.Services.AddScoped(typeof(ICrudDAO<,>), typeof(CrudDAO<,>));

            // Inject Repo
            builder.Services.AddScoped(typeof(ICrudRepo<,>), typeof(CrudRepo<,>));

            //Inject Order Service
            builder.Services.AddScoped(typeof(OrderService));

            // Inject Product Service
            builder.Services.AddScoped(typeof(ProductService));

            // Inject Email Service
            builder.Services.AddScoped(typeof(EmailService));

            // Inject User Service
            builder.Services.AddScoped(typeof(UserService));

            // Inject Cart Service
            builder.Services.AddScoped(typeof(CartService));

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Session tồn tại trong 30 phút
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });


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

            app.UseAuthentication(); // Phải trước UseAuthorization
            app.UseAuthorization();  // Phải sau UseAuthentication

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapHub<DataSignalR>("/DataUpdate"); // SignalR - config hub endpoint

            app.Run();
        }
    }
}
