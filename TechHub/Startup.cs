﻿using Microsoft.EntityFrameworkCore;
using TeachHub.Data;
using TeachHub.Services;
using Microsoft.AspNetCore.Identity;
using TeachHub.Models;

namespace TeachHub
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }


        public void ConfigureServices(IServiceCollection services)
        {
            services.AddLogging();


            services.AddDbContext<TeachHubContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Adding Identity services
            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<TeachHubContext>()
                .AddDefaultTokenProviders();


            // Configure authentication cookie settings
            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
            }); 
            
            services.AddSingleton<FirebaseService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<FirebaseService>>();
                var config = Configuration.GetSection("Firebase:StorageBucket").Value;
                return new FirebaseService(logger, config);
            });
            services.AddSingleton<StripeService>();
            services.AddControllersWithViews();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication(); // Identity authentication
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {

                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
              });

<<<<<<< HEAD
                endpoints.MapControllerRoute(
                    name: "enrollmentRoute",
                    pattern: "{controller=Enrollments}/{action=Edit}/{courseId}/{learnerId}");

            });
=======
>>>>>>> friend-repo/main
        }
    }
}
