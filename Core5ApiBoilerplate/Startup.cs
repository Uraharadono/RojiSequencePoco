using Autofac;
using Core5ApiBoilerplate.DbContext.Infrastructure;
using Core5ApiBoilerplate.Infrastructure;
using Core5ApiBoilerplate.Infrastructure.Helpers;
using Core5ApiBoilerplate.Infrastructure.Settings;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json.Serialization;

namespace Core5ApiBoilerplate
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public IWebHostEnvironment HostingEnvironment { get; }
        public IAppSettings Settings { get; }

        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            HostingEnvironment = env;
            Settings = new AppSettings(configuration);
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // I am literally registering this DB Context here, just so I can use EF Core Identity
            services.AddDbContext<Net5BoilerplateContext>(options => options.UseSqlServer(Configuration.GetConnectionString("BloggingDb")));

            // The validation system in .NET Core 3.0 and later treats non-nullable parameters or bound properties as if they had a [Required] attribute.
            // Value types such as decimal and int are non-nullable. To turn it off:
            // AddControllers(options => options.SuppressImplicitRequiredAttributeForNonNullableReferenceTypes = true)

            services.AddControllers() // we need only controllers for our api now
                // Why do we use config below? Well basically to handle DI-based activator with the AddControllersAsServices
                // Read more with examples here: https://andrewlock.net/controller-activation-and-dependency-injection-in-asp-net-core-mvc/
                .AddControllersAsServices()
                .AddNewtonsoftJson(options =>
                {
                    // To prevent "A possible object cycle was detected which is not supported" error
                    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;

                    // To get our property names serialized in the first letter lowercased
                    options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                });

            // Helpers
            IdentityHelper.ConfigureService(services);
            AuthenticationHelper.ConfigureService(services, Settings.Issuer, Settings.Audience, Settings.Key);
            // AuthenticationHelper.ConfigureServiceSimple(services, Settings.Issuer, Settings.Audience, Settings.Key);

            services.AddCors(options =>
            {
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(Settings.ClientBaseUrl)
                        .SetIsOriginAllowed(isOriginAllowed: _ => true)
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                    // .WithMethods("PUT", "DELETE", "GET", "POST");
                });
            });

            if (HostingEnvironment.IsDevelopment())
            {
                SwaggerHelper.ConfigureService(services);
            }


            //services.AddControllers();
            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Core5ApiBoilerplate", Version = "v1" });
            //});
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
            //    app.UseDeveloperExceptionPage();
            //    app.UseSwagger();
            //    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Core5ApiBoilerplate v1"));
            //}

            //app.UseHttpsRedirection();

            //app.UseRouting();

            //app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllers();
            //});


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("default"); // because of CORS issue - this is try to fix it as seen here: https://stackoverflow.com/a/56984245/4267429 - Ordering matters apparently

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            #region Swagger

            if (HostingEnvironment.IsDevelopment())
            {
                // Enable middleware to serve generated Swagger as a JSON endpoint.
                app.UseSwagger();

                // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
                // specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
                    c.RoutePrefix = "";
                });
            }

            #endregion

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            AutofacContainer.Configure(builder, Settings, Configuration);
        }
    }
}
