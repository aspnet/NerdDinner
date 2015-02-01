using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
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
        /// Gets Rsvp DB set
        /// </summary>
        IQueryable<Rsvp> Rsvp { get; }

        /// <summary>
        /// Get dinner by id asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <returns>dinner for given id</returns>
        Task<Dinner> GetDinnerAsync(int dinnerId);

        /// <summary>
        /// Get all dinners that match the filters asynchronously
        /// </summary>
        /// <param name="startDate">start date</param>
        /// <param name="endDate">end date</param>
        /// <param name="userId">user id</param>
        /// <param name="searchQuery">search query</param>
        /// <param name="sort">sort field</param>
        /// <param name="descending">sort order</param>
        /// <returns>list of dinners</returns>
        Task<List<Dinner>> GetDinnersAsync(DateTime? startDate, DateTime? endDate, int userId, string searchQuery, string sort, bool descending);

        /// <summary>
        /// Create Dinner asynchronously
        /// </summary>
        /// <param name="item">dinner to be added</param>
        /// <returns>adds dinner to db and return dinner with database generated values in it</returns>
        Task<Dinner> CreateDinnerAsync(Dinner item);

        /// <summary>
        /// Updates dinner asynchronously
        /// </summary>
        /// <param name="dinner">updated dinner item</param>
        /// <returns>updates dinner in the db and return updated dinner </returns>
        Task<Dinner> UpdateDinnerAsync(Dinner dinner);

        /// <summary>
        /// Deletes dinner asynchronously
        /// </summary>
        /// <param name="dinnerId"></param>
        /// <returns></returns>
        Task DeleteDinnerAsync(int dinnerId);

        /// <summary>
        /// Register the user for a dinner asynchronously
        /// </summary>
        /// <param name="dinner">dinner</param>
        /// <param name="userId">user Id</param>
        /// <returns>registers the user for the dinner and returns the rsvp</returns>
        Task<Rsvp> CreateRsvpAsync(Dinner dinner, int userId);

        /// <summary>
        /// Cancel registration for the user for a dinner asynchronously
        /// </summary>
        /// <param name="dinner">dinner</param>
        /// <param name="userId">user Id</param>        
        Task DeleteRsvpAsync(Dinner dinner, int userId);

        /// <summary>
        /// Find user by user name asynchronously
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns>Identity User</returns>
        Task<IdentityUser> FindUserAsync(string userName);

        /// <summary>
        /// Find external user by provider and key asynchronously
        /// </summary>
        /// <param name="provider">provider name</param>
        /// <param name="key">key value</param>
        /// <returns>Identity User</returns>
        Task<IdentityUser> FindExternalUserAsync(string provider, string key);

        /// <summary>
        /// Register user asynchronously
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Identity Result</returns>
        Task<IdentityResult> RegisterUserAsync(User user);

        /// <summary>
        /// Unsubscribe user asynchronously
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns>Identity Result</returns>
        Task<IdentityResult> UnsubscribeUserAsync(string userName);
    }
}
