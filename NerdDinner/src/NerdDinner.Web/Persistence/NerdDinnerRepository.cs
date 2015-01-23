using System;
using System.Linq;
using System.Threading.Tasks;
using NerdDinner.Web.Common;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    /// <summary>
    /// Nerd Dinner Repository
    /// </summary>
    public class NerdDinnerRepository : INerdDinnerRepository
    {
        private readonly NerdDinnerDbContext _database;

        /// <summary>
        /// Initializes a new instance of the <see cref="NerdDinnerRepository"/> class.
        /// </summary>
        /// <param name="database">database context</param>
        public NerdDinnerRepository(NerdDinnerDbContext database)
        {
            database.EnsureArgumentNotNull("database");

            _database = database;
        }

        /// <summary>
        /// Gets Dinners from the context
        /// </summary>
        public IQueryable<Dinner> Dinners => _database.Dinners;

        /// <summary>
        /// Gets Users from the context
        /// </summary>
        public IQueryable<User> Users => _database.Users;

        /// <summary>
        /// Get dinner by id asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <returns>dinner for given id</returns>
        public virtual Task<Dinner> GetDinnerAsync(long dinnerId)
        {
            return _database.Dinners.FirstOrDefaultAsync(d => d.DinnerId == dinnerId);
        }

        /// <summary>
        /// Add Dinner asynchronously
        /// </summary>
        /// <param name="dinner">dinner to be added</param>
        /// <returns>adds dinner to db and return dinner with database generated values in it</returns>
        public virtual async Task<Dinner> AddDinnerAsync(Dinner dinner)
        {
            dinner = await _database.AddAsync(dinner);
            await _database.SaveChangesAsync();

            return dinner;
        }

        /// <summary>
        /// Deletes dinner asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        public virtual async Task DeleteDinnerAsync(long dinnerId)
        {
            var dinner = await _database.Dinners.FirstOrDefaultAsync(d => d.DinnerId == dinnerId);
            if (dinner != null)
            {
                _database.Dinners.Remove(dinner);
                await _database.SaveChangesAsync();
            }

            // Else no errors - this operation is idempotent
        }

        /// <summary>
        ///  Updates dinner asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <param name="updatedDinner">updated dinner</param>
        /// <returns>updated dinner</returns>
        public virtual async Task<Dinner> UpdateDinnerAsync(long dinnerId, Dinner updatedDinner)
        {
            if (updatedDinner.DinnerId != dinnerId)
            {
                throw new ArgumentException("Dinner ID doesn't match.", "updatedDinner");
            }

            var dinner = await _database.Dinners.FirstOrDefaultAsync(d => d.DinnerId == dinnerId);
            if (dinner != null)
            {
                dinner = await _database.UpdateAsync(updatedDinner);
                await _database.SaveChangesAsync();
            }

            return dinner;
        }
    }
}
