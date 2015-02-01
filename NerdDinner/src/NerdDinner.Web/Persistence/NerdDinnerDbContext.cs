using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Metadata;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    /// <summary>
    /// Nerd Dinner Database Context
    /// </summary>
    public class NerdDinnerDbContext : IdentityDbContext<IdentityUser>
    {
        /// <summary>
        /// Gets or sets Dinners
        /// </summary>
        public virtual DbSet<Dinner> Dinners { get; set; }

        /// <summary>
        /// Gets or sets Users
        /// </summary>
        public virtual DbSet<Rsvp> Rsvp { get; set; }

        /// <summary>
        /// Override Model Creating
        /// </summary>
        /// <param name="modelBuilder">Model Builder</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Dinner>(d =>
            {
                d.Key(x => x.DinnerId);
                d.Property(x => x.DinnerId).GenerateValueOnAdd();
                d.OneToMany(x => x.Rsvps).ForeignKey(x => x.DinnerId);
            });

            modelBuilder.Entity<Rsvp>(r =>
            {
                r.Key(x => x.RsvpId);
                r.Property(x => x.RsvpId).GenerateValueOnAdd();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
