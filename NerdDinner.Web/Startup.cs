using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NerdDinner.Web.Common;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NerdDinner.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            // Setup configuration sources.
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                // For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
                builder.AddUserSecrets();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add EF services to the services container.
            services.AddEntityFrameworkSqlServer()
                    .AddDbContext<NerdDinnerDbContext>(options =>
                            options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddScoped<INerdDinnerRepository, NerdDinnerRepository>();

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<NerdDinnerDbContext>()
                .AddDefaultTokenProviders();

            //services.ConfigureFacebookAuthentication(options =>
            //{
            //    options.ClientId = Configuration["Authentication:Facebook:AppId"];
            //    options.ClientSecret = Configuration["Authentication:Facebook:AppSecret"];
            //});

            //services.ConfigureGoogleAuthentication(options =>
            //{
            //    options.ClientId = Configuration["Authentication:Google:AppId"];
            //    options.ClientSecret = Configuration["Authentication:Google:AppSecret"];
            //});

            //services.ConfigureTwitterAuthentication(options =>
            //{
            //    options.ConsumerKey = Configuration["Authentication:Twitter:AppId"];
            //    options.ConsumerSecret = Configuration["Authentication:Twitter:AppSecret"];
            //});

            //services.ConfigureMicrosoftAccountAuthentication(options =>
            //{
            //    options.Caption = "MicrosoftAccount - Requires project changes";
            //    options.ClientId = Configuration["Authentication:Microsoft:AppId"];
            //    options.ClientSecret = Configuration["Authentication:Microsoft:AppSecret"];
            //});

            // Add MVC services to the services container.
            services.AddMvc().AddMvcOptions(options =>
            {
                var settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var formatter = new JsonOutputFormatter { SerializerSettings = settings };

                options.OutputFormatters.Insert(0, formatter);

                // Add validation filters
                options.Filters.Add(new ValidateModelFilterAttribute());
            });
        }

        // Configure is called after ConfigureServices is called.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();

            //app.UseFacebookAuthentication();
            //app.UseGoogleAuthentication();
            //app.UseMicrosoftAccountAuthentication();
            //app.UseTwitterAuthentication();

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            // Call EnsureCreated to get schema created. 
            var serviceScopeFactory = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>();
            using (var serviceScope = serviceScopeFactory.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetService<NerdDinnerDbContext>();
                dbContext.Database.EnsureCreated();
            }
        }
    }
}
