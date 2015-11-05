using Microsoft.AspNet.Authentication.Facebook;
using Microsoft.AspNet.Authentication.MicrosoftAccount;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc;
using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using Microsoft.Framework.Logging;
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
            var configuration = new Configuration()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                configuration.AddUserSecrets();
            }
            configuration.AddEnvironmentVariables();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Add EF services to the services container.
            services.AddEntityFramework()
                .AddSqlServer()
                .AddDbContext<NerdDinnerDbContext>(options =>
                    options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]));

            services.AddScoped<INerdDinnerRepository, NerdDinnerRepository>();

            // Add Identity services to the services container.
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<NerdDinnerDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureFacebookAuthentication(options =>
            {
                options.ClientId = Configuration["Authentication:Facebook:AppId"];
                options.ClientSecret = Configuration["Authentication:Facebook:AppSecret"];
            });

            services.ConfigureGoogleAuthentication(options =>
            {
                options.ClientId = Configuration["Authentication:Google:AppId"];
                options.ClientSecret = Configuration["Authentication:Google:AppSecret"];
            });

            services.ConfigureTwitterAuthentication(options =>
            {
                options.ConsumerKey = Configuration["Authentication:Twitter:AppId"];
                options.ConsumerSecret = Configuration["Authentication:Twitter:AppSecret"];
            });

            //services.ConfigureMicrosoftAccountAuthentication(options =>
            //{
            //    options.Caption = "MicrosoftAccount - Requires project changes";
            //    options.ClientId = Configuration["Authentication:Microsoft:AppId"];
            //    options.ClientSecret = Configuration["Authentication:Microsoft:AppSecret"];
            //});

            // Add MVC services to the services container.
            services.AddMvc().Configure<MvcOptions>(options =>
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
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerfactory)
        {
            // Configure the HTTP request pipeline.

            // Add the console logger.
            loggerfactory.AddConsole(minLevel: LogLevel.Warning);

            // Add the following to the request pipeline only in development environment.
            if (env.IsEnvironment("Development"))
            {
                app.UseErrorPage(ErrorPageOptions.ShowAll);
                app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                app.UseErrorHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();

            app.UseFacebookAuthentication();
            app.UseGoogleAuthentication();
            //app.UseMicrosoftAccountAuthentication();
            app.UseTwitterAuthentication();

            // Add MVC to the request pipeline.
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });
            });

            //SampleData.InitializeNerdDinner(app.ApplicationServices).Wait();
        }
    }
}
