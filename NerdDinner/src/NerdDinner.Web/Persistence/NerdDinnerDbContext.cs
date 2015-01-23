using Microsoft.Data.Entity;
using Microsoft.Framework.ConfigurationModel;
using NerdDinner.Web.Common;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    /// <summary>
    /// Nerd Dinner Database Context
    /// </summary>
    public class NerdDinnerDbContext : DbContext
    {
        /// <summary>
        /// Configuration
        /// </summary>
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Gets or sets Dinners
        /// </summary>
        public virtual DbSet<Dinner> Dinners { get; }

        /// <summary>
        /// Gets or sets Users
        /// </summary>
        public virtual DbSet<User> Users { get; }

        /// <summary>
        /// Initializes a new instance of the type <see cref="NerdDinnerDbContext"/>
        /// </summary>
        /// <param name="configuration"></param>
        public NerdDinnerDbContext(IConfiguration configuration)
        {
            configuration.EnsureArgumentNotNull("configuration");
            _configuration = configuration;
        }

        /// <summary>
        /// On Configuring function is used set connection string
        /// </summary>
        /// <param name="options">Db Context options</param>
        protected override void OnConfiguring(DbContextOptions options)
        {
            options.UseSQLite(_configuration.Get("Data:DefaultConnection:ConnectionString"));
        }
    }
}
