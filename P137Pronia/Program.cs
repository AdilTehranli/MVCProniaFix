using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using P137Pronia.DataAccess;
using P137Pronia.Models;
using P137Pronia.Services;

namespace P137Pronia
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddService();
            builder.Services.AddSession();

            builder.Services.AddDbContext<ProniaDbContext>(opt =>
            {
                opt.UseSqlServer(builder.Configuration["ConnectionStrings:MSSQL"], opt =>
                {
                    opt.EnableRetryOnFailure();
                });
                //opt.UseNpgsql();
            }).AddIdentity<AppUser, IdentityRole>(opt =>
            {
                opt.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
                opt.Password.RequireNonAlphanumeric = false;
                opt.Password.RequiredLength = 8;
                opt.Lockout.AllowedForNewUsers = true;
                opt.Lockout.MaxFailedAccessAttempts = 3;
                opt.SignIn.RequireConfirmedEmail = false;
            }).AddDefaultTokenProviders().AddEntityFrameworkStores<ProniaDbContext>();
            builder.Services.AddHttpContextAccessor();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Shared/Error");
                app.UseHsts();
            }
            if (app.Environment.IsProduction())
            {
                app.UseStatusCodePagesWithRedirects("~/error.html");
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseSession();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                  name: "areas",
                  pattern: "{area:exists}/{controller=Slider}/{action=Index}/{id?}"
                );
            });
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");


            app.Run();
        }
    }
}