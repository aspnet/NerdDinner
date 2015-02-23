using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    public class NerdDinnerRepository : INerdDinnerRepository
    {
        private readonly NerdDinnerDbContext _database;

        private readonly UserManager<IdentityUser> _userManager;

        private readonly SignInManager<IdentityUser> _signInManager;

        public NerdDinnerRepository(NerdDinnerDbContext database, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _database = database;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public IQueryable<Dinner> Dinners => _database.Dinners;

        public IQueryable<Rsvp> Rsvp => _database.Rsvp;

        public virtual async Task<Dinner> GetDinnerAsync(int dinnerId)
        {
            try
            {
                return await _database.Dinners
                    .Include(d => d.Rsvps)
                    .SingleOrDefaultAsync(d => d.DinnerId == dinnerId);
            }
            catch (AggregateException)
            {
                return null;
            }
        }

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

            query = ApplyDinnerSort(query, sort, descending);

            return await query.ToListAsync();
        }

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

        public virtual async Task<Dinner> UpdateDinnerAsync(Dinner dinner)
        {
            _database.Update(dinner);
            await _database.SaveChangesAsync();
            return dinner;
        }

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

        public async Task<IdentityUser> FindUserAsync(string userName)
        {
            return await _userManager.FindByNameAsync(userName);
        }

        public async Task<IdentityUser> FindExternalUserAsync(string provider, string  key)
        {
            return await _userManager.FindByLoginAsync(provider, key);
        }

        public async Task<IdentityResult> RegisterUserAsync(User user)
        {
            var identityUser = new IdentityUser
            {
                UserName = user.UserName                
            };

            return await _userManager.CreateAsync(identityUser, user.Password);
        }

        public async Task<IdentityResult> UnsubscribeUserAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);
            return await _userManager.DeleteAsync(user);
        }

        private IQueryable<Dinner> ApplyDinnerSort(IQueryable<Dinner> query, string sort, bool descending)
        {
            // Default to sort by dinner Id
            query = descending ? query.OrderByDescending(d => d.DinnerId) : query.OrderBy(d => d.DinnerId);

            if (string.Equals(sort, "Title", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.Title) : query.OrderBy(d => d.Title);
            }
            else if (string.Equals(sort, "EventDate", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.EventDate) : query.OrderBy(d => d.EventDate);
            }
            else if (string.Equals(sort, "UserId", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.UserId) : query.OrderBy(d => d.UserId);
            }
            else if (string.Equals(sort, "HostedByName", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.HostedByName) : query.OrderBy(d => d.HostedByName);
            }

            return query;
        }
    }
}
