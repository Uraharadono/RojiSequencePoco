using System;
using Core5ApiBoilerplate.DbContext.Entities.Identity;
using Core5ApiBoilerplate.DbContext.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Core5ApiBoilerplate.Infrastructure.Helpers
{
    public class IdentityHelper
    {
        public static void ConfigureService(IServiceCollection services)
        {
            //services.AddIdentityCore<ApplicationUser>()
            //    .AddRoles<ApplicationRole>()
            //    .AddEntityFrameworkStores<Net5BoilerplateContext>()
            //    //.AddRoleStore<ApplicationRoleStore>()
            //    //.AddUserStore<ApplicationUserStore>()
            //    .AddDefaultTokenProviders();

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<Net5BoilerplateContext>()
                .AddDefaultTokenProviders();

            services.Configure<IdentityOptions>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequiredLength = 8;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 6;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 10;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.RequireUniqueEmail = true;
            });
        }
    }
}
