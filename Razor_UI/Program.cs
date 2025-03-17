using BLL.Interfaces;
using BLL.Services;
using DataAccess.DAOs;
using DataAccess.Models;
using Microsoft.EntityFrameworkCore;
using Razor_UI.SignalRHub;

namespace Razor_UI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();

            builder.Services.AddDbContext<BakeryShopDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Inject Dao
            builder.Services.AddScoped(typeof(ICrudDAO<,>), typeof(CrudDAO<,>));

            // Inject Repo
            builder.Services.AddScoped(typeof(ICrudRepo<,>), typeof(CrudRepo<,>));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapHub<DataSignalR>("/DataUpdate"); // SignalR - config hub endpoint

            app.Run();
        }
    }
}
