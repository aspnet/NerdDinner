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

        public void Configure(IApplicationBuilder app)
        {
            app.UseServices(services =>
            {
                // Use in memory store
                services
                    .AddEntityFramework()
                    .AddInMemoryStore()
                    .AddDbContext<NerdDinnerDbContext>();

                // services
                //    .AddEntityFramework()
                //    .AddSQLite()
                //    .AddDbContext<NerdDinnerDbContext>(options =>
                //    {
                //        options.UseSQLite(Configuration.Get("Data:DefaultConnection:ConnectionString"));
                //    });

                services.AddScoped<INerdDinnerRepository, NerdDinnerRepository>();
                services
                    .AddIdentity<IdentityUser, IdentityRole>()
                    .AddEntityFrameworkStores<NerdDinnerDbContext>()
                    .AddDefaultTokenProviders();

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
            });

            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    null,
                    "api/{controller}/{id?}");
            });

            //Populates the sample data
            SampleData.InitializeNerdDinner(app.ApplicationServices).Wait();
        }
    }
}
