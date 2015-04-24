using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    public interface INerdDinnerRepository
    {
        IQueryable<Dinner> Dinners { get; }

        IQueryable<Rsvp> Rsvp { get; }

        Task<Dinner> GetDinnerAsync(int dinnerId);
        
        Task<List<Dinner>> GetDinnersAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, double? lat, double? lng);

        Task<List<Dinner>> GetPopularDinnersAsync();

        Task<Dinner> CreateDinnerAsync(Dinner item);

        Task<Dinner> UpdateDinnerAsync(Dinner dinner);

        Task DeleteDinnerAsync(int dinnerId);

        Task<Rsvp> CreateRsvpAsync(Dinner dinner, string userName);

        Task DeleteRsvpAsync(Dinner dinner, string userName);
    }
}
