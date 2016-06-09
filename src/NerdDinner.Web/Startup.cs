using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Diagnostics.Entity;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Mvc.Formatters;
using Microsoft.Data.Entity;
using Microsoft.Framework.Configuration;
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
            var configurationBuilder = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddJsonFile($"config.{env.EnvironmentName}.json", optional: true);

            if (env.IsEnvironment("Development"))
            {
                configurationBuilder.AddUserSecrets();
            }

            configurationBuilder.AddEnvironmentVariables();
            Configuration = configurationBuilder.Build();
        }

        private IConfiguration Configuration { get; }

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

            services.AddAuthentication();

            // Add MVC services to the services container.
            services.AddMvc(options =>
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
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage(DatabaseErrorPageOptions.ShowAll);
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseIISPlatformHandler();
            app.UseStaticFiles();
            app.UseIdentity();

            app.UseFacebookAuthentication(options =>
                                          {
                                              options.ClientId = Configuration["Authentication:Facebook:AppId"];
                                              options.ClientSecret = Configuration["Authentication:Facebook:AppSecret"];
                                          });
            app.UseGoogleAuthentication(options =>
                                        {
                                            options.ClientId = Configuration["Authentication:Google:AppId"];
                                            options.ClientSecret = Configuration["Authentication:Google:AppSecret"];
                                        });
            //app.UseMicrosoftAccountAuthentication(options =>
            //                                      {
            //                                          options.ClientId = Configuration["Authentication:Microsoft:AppId"];
            //                                          options.ClientSecret = Configuration["Authentication:Microsoft:AppSecret"];
            //                                      });
            app.UseTwitterAuthentication(options =>
                                         {
                                             options.ConsumerKey = Configuration["Authentication:Twitter:AppId"];
                                             options.ConsumerSecret = Configuration["Authentication:Twitter:AppSecret"];
                                         });

            // Add MVC to the request pipeline.
            app.UseMvcWithDefaultRoute();

            //SampleData.InitializeNerdDinner(app.ApplicationServices).Wait();
        }
    }
}
