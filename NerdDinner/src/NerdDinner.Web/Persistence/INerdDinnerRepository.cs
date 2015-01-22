using System.Linq;
using System.Threading.Tasks;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    /// <summary>
    /// Nerd Dinner Repository Interface
    /// </summary>
    public interface INerdDinnerRepository
    {
        /// <summary>
        /// Gets Dinners DB set
        /// </summary>
        IQueryable<Dinner> Dinners { get; }

        /// <summary>
        /// Gets Users DB set
        /// </summary>
        IQueryable<User> Users { get; }

        /// <summary>
        /// Get dinner by id
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <returns>dinner for given id</returns>
        Task<Dinner> GetDinnerAsync(long dinnerId);

        /// <summary>
        /// Add Dinner asynchronously
        /// </summary>
        /// <param name="item">dinner to be added</param>
        /// <returns>adds dinner to db and return dinner with database generated values in it</returns>
        Task<Dinner> AddDinnerAsync(Dinner item);

        /// <summary>
        /// Deletes dinner asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner to be deleted</param>
        /// <returns>deletes dinner from the db</returns>
        Task DeleteDinnerAsync(long dinnerId);

        /// <summary>
        /// Updates dinner asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <param name="updatedDinner">updated dinner item</param>
        /// <returns>updates dinner in the db and return updated dinner </returns>
        Task<Dinner> UpdateDinnerAsync(long dinnerId, Dinner updatedDinner);
    }
}
