using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Mvc;
using Microsoft.AspNet.Routing;
using Microsoft.Framework.ConfigurationModel;
using Microsoft.Framework.DependencyInjection;
using NerdDinner.Web.Common;
using NerdDinner.Web.Persistence;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace NerdDinner.Web
{
    /// <summary>
    /// Startup Class
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        public Startup()
        {
            // Setup configuration sources.
            Configuration = new Configuration().AddJsonFile("config.json");
        }

        /// <summary>
        /// Gets or sets Configuration
        /// </summary>
        public IConfiguration Configuration { get; set; }

        /// <summary>
        /// Configure all settings
        /// </summary>
        /// <param name="app">application builder</param>
        public void Configure(IApplicationBuilder app)
        {
            app.UseServices(services =>
            {
                // Add EF services to the services container and SQLite
                services
                    .AddEntityFramework()
                    .AddSQLite();

                services.AddScoped<INerdDinnerRepository, NerdDinnerRepository>();
                services.AddScoped<NerdDinnerDbContext>();
                services.AddScoped<IConfiguration>(s => Configuration);

                // Add MVC services to the services container
                services.AddMvc().Configure<MvcOptions>(options =>
                {
                    // If you want to restrict the output to only JSON all the time, then uncomment the following line.
                    // options.OutputFormatters.RemoveAll(formatters => formatters.Instance.GetType() == typeof(XmlDataContractSerializerOutputFormatter));

                    // TODO: Find out if there is a situation where the below method might return -1.
                    var position = options.OutputFormatters.FindIndex(f => f.Instance is JsonOutputFormatter);
                    var settings = new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented,
                        ContractResolver = new CamelCasePropertyNamesContractResolver()
                    };

                    var formatter = new JsonOutputFormatter { SerializerSettings = settings };

                    options.OutputFormatters.RemoveAt(position);
                    options.OutputFormatters.Insert(position, formatter);

                    // Add validation and exception filters
                    options.Filters.Add(new ValidateModelFilter());
                    options.Filters.Add(new GlobalExceptionFilter());
                });
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    null,
                    "api/{controller}/{id?}");
            });
        }
    }
}
