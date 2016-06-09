using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    public class NerdDinnerDbContext : IdentityDbContext<ApplicationUser>
    {
        public virtual DbSet<Dinner> Dinners { get; set; }

        public virtual DbSet<Rsvp> Rsvp { get; set; }

        public NerdDinnerDbContext()
        {
            Database.EnsureCreatedAsync().Wait();
        }
    }
}
