using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using NerdDinner.Web.Common;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NerdDinner.Web
{
    public class Startup
    {
        public Startup()
        {
            Configuration = new Configuration().AddJsonFile("config.json");
        }

        public IConfiguration Configuration { get; set; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Use in memory store
            //services
            //    .AddEntityFramework()
            //    .AddInMemoryStore()
            //    .AddDbContext<NerdDinnerDbContext>();

            services
               .AddEntityFramework(Configuration)
               .AddSqlServer()
               .AddDbContext<NerdDinnerDbContext>();

            services.AddScoped<INerdDinnerRepository, NerdDinnerRepository>();
            services
                .AddIdentity<ApplicationUser, IdentityRole>(Configuration)
                .AddEntityFrameworkStores<NerdDinnerDbContext>()
                .AddDefaultTokenProviders();

            services.ConfigureFacebookAuthentication(options =>
            {
                options.ClientId = "1628974507321076";
                options.ClientSecret = "7c8680db91b175bbe97f77a051380546";
            });

            services.ConfigureGoogleAuthentication(options =>
            {
                options.ClientId = "135363840336-ervf2q3393fae41b023f8vmvnilthkd3.apps.googleusercontent.com";
                options.ClientSecret = "yJYn7pDolux-4ObEqkMkXbdb";
            });

            services.ConfigureTwitterAuthentication(options =>
            {
                options.ConsumerKey = "9J3j3pSwgbWkgPFH7nAf0Spam";
                options.ConsumerSecret = "jUBYkQuBFyqp7G3CUB9SW3AfflFr9z3oQBiNvumYy87Al0W4h8";
            });

            services.ConfigureMicrosoftAccountAuthentication(options =>
            {
                options.Caption = "MicrosoftAccount - Requires project changes";
                options.ClientId = "000000004012C08A";
                options.ClientSecret = "GaMQ2hCnqAC6EcDLnXsAeBVIJOLmeutL";
            });

            //services.Configure<AuthorizationOptions>(options =>
            //{
            //    options.AddPolicy("CanEdit", new AuthorizationPolicyBuilder().RequiresClaim("CanEdit", "Allowed").Build());
            //});

            services.AddMvc().Configure<MvcOptions>(options =>
            {
                var position = options.OutputFormatters.FindIndex(f => f.Instance is JsonOutputFormatter);
                var settings = new JsonSerializerSettings()
                {
                    Formatting = Formatting.Indented,
                    ContractResolver = new CamelCasePropertyNamesContractResolver()
                };

                var formatter = new JsonOutputFormatter { SerializerSettings = settings };

                options.OutputFormatters.RemoveAt(position);
                options.OutputFormatters.Insert(position, formatter);

                // Add validation filters
                options.Filters.Add(new ValidateModelFilter());
            });
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseStaticFiles();
            app.UseIdentity();
            app.UseFacebookAuthentication();
            app.UseGoogleAuthentication();
            app.UseTwitterAuthentication();
            app.UseMicrosoftAccountAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action}/{id?}",
                    defaults: new { controller = "Home", action = "Index" });

                routes.MapRoute(
                    null,
                    "api/{controller}/{id?}");
            });

            //Populates the sample data
             //SampleData.InitializeNerdDinner(app.ApplicationServices).Wait();
        }
    }
}
