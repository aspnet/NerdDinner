using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    /// <summary>
    /// Nerd Dinner Repository
    /// </summary>
    public class NerdDinnerRepository : INerdDinnerRepository
    {
        private readonly NerdDinnerDbContext _database;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="NerdDinnerRepository"/> class.
        /// </summary>
        /// <param name="database">database context</param>
        /// <param name="userManager">user manager</param>
        /// <param name="signInManager">signin manager</param>
        public NerdDinnerRepository(NerdDinnerDbContext database, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            if (database == null)
            {
                throw new ArgumentNullException("database");
            }

            if (userManager == null)
            {
                throw new ArgumentNullException("userManager");
            }

            if (signInManager == null)
            {
                throw new ArgumentNullException("signInManager");
            }

            _database = database;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        /// <summary>
        /// Gets Dinners from the context
        /// </summary>
        public IQueryable<Dinner> Dinners => _database.Dinners;

        /// <summary>
        /// Gets Rsvp from the context
        /// </summary>
        public IQueryable<Rsvp> Rsvp => _database.Rsvp;

        /// <summary>
        /// Get dinner by id asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <returns>dinner for given id</returns>
        public virtual async Task<Dinner> GetDinnerAsync(int dinnerId)
        {
            try
            {
                return await _database.Dinners
                    .Include(d => d.Rsvps)
                    .SingleOrDefaultAsync(d => d.DinnerId == dinnerId);
            }
            // Include throws error if dinner does not exist
            // https://github.com/aspnet/EntityFramework/issues/1511
            catch (AggregateException)
            {
                return null;
            }
        }

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
        public virtual async Task<List<Dinner>> GetDinnersAsync(DateTime? startDate, DateTime? endDate, int userId, string searchQuery, string sort, bool descending)
        {
            var query = _database.Dinners.AsQueryable();

            if (userId != 0)
            {
                query = query.Where(d => d.UserId == userId);
            }

            if (startDate != null)
            {
                query = query.Where(d => d.EventDate >= startDate);
            }

            if (endDate != null)
            {
                query = query.Where(d => d.EventDate <= endDate);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(
                    d => d.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1 ||
                    d.Description.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1);
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = ApplySort(query, sort, descending);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Add Dinner asynchronously
        /// </summary>
        /// <param name="dinner">dinner to be added</param>
        /// <returns>adds dinner to db and return dinner with database generated values in it</returns>
        public virtual async Task<Dinner> CreateDinnerAsync(Dinner dinner)
        {
            var rsvp = new Rsvp
            {
                UserId = dinner.UserId
            };

            dinner.Rsvps = new List<Rsvp> {rsvp};

            _database.Add(dinner);
            _database.Add(rsvp);
            await _database.SaveChangesAsync();

            return dinner;
        }

        /// <summary>
        ///  Updates dinner asynchronously
        /// </summary>
        /// <param name="dinner">updated dinner</param>
        /// <returns>updated dinner</returns>
        public virtual async Task<Dinner> UpdateDinnerAsync(Dinner dinner)
        {
            _database.Update(dinner);
            await _database.SaveChangesAsync();
            return dinner;
        }

        /// <summary>
        /// Deletes dinner asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        public virtual async Task DeleteDinnerAsync(int dinnerId)
        {
            var dinner = await GetDinnerAsync(dinnerId);
            if (dinner != null)
            {
                foreach(Rsvp rsvp in dinner.Rsvps)
                {
                    _database.Rsvp.Remove(rsvp);
                }

                _database.Dinners.Remove(dinner);

                await _database.SaveChangesAsync();
            }

            // Else no errors - this operation is idempotent
        }

        /// <summary>
        /// Register the user for a dinner asynchronously
        /// </summary>
        /// <param name="dinner">dinner</param>
        /// <param name="userId">user Id</param>
        /// <returns>registers the user for the dinner</returns>
        public virtual async Task<Rsvp> CreateRsvpAsync(Dinner dinner, int userId)
        {
            Rsvp rsvp = null;
            if (dinner != null)
            {
                if (dinner.IsUserRegistered(userId))
                {
                    rsvp = dinner.Rsvps.SingleOrDefault(r => r.UserId == userId);
                }
                else
                {
                    rsvp = new Rsvp
                    {
                        UserId = userId
                    };

                    dinner.Rsvps.Add(rsvp);
                    _database.Add(rsvp);
                    await _database.SaveChangesAsync();
                }
            }

            return rsvp;
        }

        /// <summary>
        /// Cancel registration for the user for a dinner asynchronously
        /// </summary>
        /// <param name="dinner">dinner id</param>
        /// <param name="userId">user Id</param>
        public virtual async Task DeleteRsvpAsync(Dinner dinner, int userId)
        {
            var rsvp = dinner?.Rsvps.SingleOrDefault(r => r.UserId == userId);
            if (rsvp != null)
            {
                _database.Rsvp.Remove(rsvp);
                await _database.SaveChangesAsync();
            };

            // Else no errors - this operation is idempotent
        }

        /// <summary>
        /// Find user by user name asynchronously
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns>Identity User</returns>
        public async Task<IdentityUser> FindUserAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        /// <summary>
        /// Find external user by provider and key asynchronously
        /// </summary>
        /// <param name="provider">provider name</param>
        /// <param name="key">key value</param>
        /// <returns>Identity User</returns>
        public async Task<IdentityUser> FindExternalUserAsync(string provider, string  key)
        {
            return await _userManager.FindByLoginAsync(provider, key);
        }

        /// <summary>
        /// Register user asynchronously
        /// </summary>
        /// <param name="user">User</param>
        /// <returns>Identity Result</returns>
        public async Task<IdentityResult> RegisterUserAsync(User user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.UserName                
            };

            return await _userManager.CreateAsync(identityUser, user.Password);
        }

        /// <summary>
        /// Unsubscribe user asynchronously
        /// </summary>
        /// <param name="userName">user name</param>
        /// <returns>Identity Result</returns>
        public async Task<IdentityResult> UnsubscribeUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return await _userManager.DeleteAsync(user);
        }

        /// <summary>
        /// Apply sort
        /// </summary>
        /// <param name="query">The query</param>
        /// <param name="sort">sort field</param>
        /// <param name="descending">sort order</param>
        /// <returns>updated query</returns>
        private IQueryable<Dinner> ApplySort(IQueryable<Dinner> query, string sort, bool descending)
        {
            // Default to sort by dinner Id
            query = descending ? query.OrderByDescending(d => d.DinnerId) : query.OrderBy(d => d.DinnerId);

            if (sort.Equals("Title", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.Title) : query.OrderBy(d => d.Title);
            }
            else if (sort.Equals("EventDate", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.EventDate) : query.OrderBy(d => d.EventDate);
            }
            else if (sort.Equals("UserId", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.UserId) : query.OrderBy(d => d.UserId);
            }
            else if (sort.Equals("HostedByName", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.HostedByName) : query.OrderBy(d => d.HostedByName);
            }

            return query;
        }
    }
}
