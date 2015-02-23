using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    public interface INerdDinnerRepository
    {
        IQueryable<Dinner> Dinners { get; }

        IQueryable<Rsvp> Rsvp { get; }

        Task<Dinner> GetDinnerAsync(int dinnerId);

        Task<List<Dinner>> GetDinnersAsync(DateTime? startDate, DateTime? endDate, int userId, string searchQuery, string sort, bool descending);

        Task<Dinner> CreateDinnerAsync(Dinner item);

        Task<Dinner> UpdateDinnerAsync(Dinner dinner);

        Task DeleteDinnerAsync(int dinnerId);

        Task<Rsvp> CreateRsvpAsync(Dinner dinner, int userId);

        Task DeleteRsvpAsync(Dinner dinner, int userId);

        Task<IdentityUser> FindUserAsync(string userName);

        Task<IdentityUser> FindExternalUserAsync(string provider, string key);

        Task<IdentityResult> RegisterUserAsync(User user);

        Task<IdentityResult> UnsubscribeUserAsync(string userName);
    }
}
