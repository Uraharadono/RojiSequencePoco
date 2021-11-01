using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Core5ApiBoilerplate.Controllers;
using Core5ApiBoilerplate.DbContext.Infrastructure;
using Core5ApiBoilerplate.Infrastructure.Repository;
using Core5ApiBoilerplate.Infrastructure.Services;
using Core5ApiBoilerplate.Infrastructure.Settings;
using Core5ApiBoilerplate.Services.Blog;
using Core5ApiBoilerplate.Utility.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Core5ApiBoilerplate.Infrastructure
{
    public static class AutofacContainer
    {
        public static void Configure(ContainerBuilder builder, IAppSettings settings, IConfiguration configuration)
        {
            var connection = configuration.GetConnectionString("BloggingDb");
            var injectableAssemblies = GetInjectableAssemblies().ToArray();

            // Register appsettings.json properties
            builder.Register(c => settings)
                .AsImplementedInterfaces()
                .SingleInstance();


            // ASP.NET Core apps access HttpContext through the IHttpContextAccessor interface and its default implementation HttpContextAccessor.
            // It's only necessary to use IHttpContextAccessor when you need access to the HttpContext inside a service.
            builder.RegisterType<HttpContextAccessor>()
                .As<IHttpContextAccessor>()
                .SingleInstance();

            // Identity (not needed as we have it registered in Startup)
            // builder.RegisterType<UserStore<ApplicationUser>>().As<IUserStore<ApplicationUser>>();
            // builder.RegisterType<RoleStore<IdentityRole>>().As<IRoleStore<IdentityRole>>();
            // builder.RegisterType<RoleManager<ApplicationRole>>().As<IRole<IdentityRole>>();
            //builder.RegisterType<ApplicationUserManager>();
            //builder.RegisterType<ApplicationRoleManager>();


            // Unit of work and Repository
            builder.Register(c => new UnitOfWork(Net5BoilerplateContext.Create(connection, settings.OrganizationType)))
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();
            builder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();


            // TODO: // Emails 
            //builder.RegisterType<EmailService>()
            //    .As<IEmailService>()
            //    .SingleInstance();

            // Controllers
            builder.RegisterAssemblyTypes(typeof(BlogsController).Assembly)
                .OnActivating(e => PrepareController(e.Context, e.Instance as BaseController));

            // Inherited infrastructure instances
            builder.RegisterAssemblyTypes(injectableAssemblies).Where(IsInjectable)
                .AsImplementedInterfaces()
                .PropertiesAutowired()
                .InstancePerLifetimeScope();
        }

        private static void PrepareController(IComponentContext ctx, BaseController c)
        {
            if (ctx == null || c == null)
                return;
            
            c.UnitOfWork = ctx.Resolve<IUnitOfWork>();
        }

        private static IEnumerable<Assembly> GetInjectableAssemblies()
        {
            yield return Assembly.GetAssembly(typeof(AppException)); // Infrastructure
            yield return Assembly.GetAssembly(typeof(BlogService)); // Services
        }

        private static bool IsInjectable(Type t)
        {
            var interfaces = t.GetInterfaces();
            var injectable = interfaces.Any(i => 
                i.IsAssignableFrom(typeof(IService)) ||
                i.IsAssignableFrom(typeof(IRepository))
            );
            return injectable;
        }
    }
}
