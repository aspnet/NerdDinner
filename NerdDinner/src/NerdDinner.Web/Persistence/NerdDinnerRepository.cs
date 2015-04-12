using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.Http.Security;
using Microsoft.AspNet.Identity;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    public class NerdDinnerRepository : INerdDinnerRepository
    {
        private readonly NerdDinnerDbContext _database;

        private readonly UserManager<ApplicationUser> _userManager;

        private readonly SignInManager<ApplicationUser> _signInManager;

        public NerdDinnerRepository(NerdDinnerDbContext database, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _database = database;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public UserManager<ApplicationUser> UserManager => _userManager;

        public SignInManager<ApplicationUser> SignInManager => _signInManager;

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

        public virtual async Task<List<Dinner>> GetDinnersAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending)
        {
            var query = _database.Dinners.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(d => string.Equals(d.UserName, userName, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate != null)
            {
                query = query.Where(d => d.EventDate >= startDate);
            }
            else
            {
                query = query.Where(d => d.EventDate >= DateTime.Now);
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

        public virtual async Task<List<Dinner>> GetPopularDinnersAsync()
        {
            var result = await _database.Dinners
                .Include(d => d.Rsvps)
                .ToListAsync();

            return result
                .OrderByDescending(d => d.Rsvps.Count)
                .Take(5)
                .ToList();
        }

        public virtual async Task<Dinner> CreateDinnerAsync(Dinner dinner)
        {
            var rsvp = new Rsvp
            {
                UserName = dinner.UserName
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

        public virtual async Task<Rsvp> CreateRsvpAsync(Dinner dinner, string userName)
        {
            Rsvp rsvp = null;
            if (dinner != null)
            {
                if (dinner.IsUserRegistered(userName))
                {
                    rsvp = dinner.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    rsvp = new Rsvp
                    {
                        UserName = userName
                    };

                    dinner.Rsvps.Add(rsvp);
                    _database.Add(rsvp);
                    await _database.SaveChangesAsync();
                }
            }

            return rsvp;
        }

        public virtual async Task DeleteRsvpAsync(Dinner dinner, string userName)
        {
            var rsvp = dinner?.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
            if (rsvp != null)
            {
                _database.Rsvp.Remove(rsvp);
                await _database.SaveChangesAsync();
            };

            // Else no errors - this operation is idempotent
        }

        public virtual async Task<ApplicationUser> FindUserAsync(string userName)
        {
            return await UserManager.FindByNameAsync(userName);
        }

        //public virtual async Task<IdentityUser> FindExternalUserAsync(string provider, string  key)
        //{
        //    return await UserManager.FindByLoginAsync(provider, key);
        //}

        //public virtual async Task<IdentityResult> RegisterUserAsync(User user)
        //{
        //    var identityUser = new IdentityUser
        //    {
        //        UserName = user.UserName                
        //    };

        //    return await UserManager.CreateAsync(identityUser, user.Password);
        //}

        //public virtual async Task<IdentityResult> UnsubscribeUserAsync(string userName)
        //{
        //    var user = await UserManager.FindByNameAsync(userName);
        //    return await UserManager.DeleteAsync(user);
        //}

        //public virtual async Task<ExternalLoginInfo> GetExternalLoginInfoAsync(CancellationToken token)
        //{
        //    return await SignInManager.GetExternalLoginInfoAsync(cancellationToken: token);
        //}

        //public virtual async Task<SignInResult> ExternalLoginSignInAsync(string provider, string key, CancellationToken token)
        //{
        //    return await SignInManager.ExternalLoginSignInAsync(provider, key, isPersistent: false, cancellationToken: token);
        //}

        //public virtual AuthenticationProperties ConfigureExternalAuth(string provider, string redirectUrl)
        //{
        //    return SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //}

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
            else if (string.Equals(sort, "UserName", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.UserName) : query.OrderBy(d => d.UserName);
            }

            return query;
        }
    }
}
