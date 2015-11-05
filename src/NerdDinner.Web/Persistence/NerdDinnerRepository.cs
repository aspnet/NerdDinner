using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdDinner.Web.Models;

namespace NerdDinner.Web.Persistence
{
    public class NerdDinnerRepository : INerdDinnerRepository
    {
        private readonly NerdDinnerDbContext _database;

        public NerdDinnerRepository(NerdDinnerDbContext database)
        {
            _database = database;
        }

        public IQueryable<Dinner> Dinners => _database.Dinners;

        public IQueryable<Rsvp> Rsvp => _database.Rsvp;

        public virtual async Task<Dinner> GetDinnerAsync(int dinnerId)
        {
            return await _database.Dinners
                .Include(d => d.Rsvps)
                .SingleOrDefaultAsync(d => d.DinnerId == dinnerId);
        }        

        public virtual async Task<List<Dinner>> GetDinnersAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, double? lat, double? lng, int? pageIndex, int? pageSize)
        {
            var query = _database.Dinners.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(d => string.Equals(d.UserName, userName, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue)
            {
                query = query.Where(d => d.EventDate >= startDate.Value);
            }
            else
            {
                query = query.Where(d => d.EventDate >= DateTime.Now);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.EventDate <= endDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(
                    d => d.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1 ||
                    d.Description.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1);
            }

            if (lat.HasValue)
            {
                query = query.Where(d => d.Latitude == lat.Value);
            }

            if (lng.HasValue)
            {
                query = query.Where(d => d.Longitude == lng.Value);
            }

            query = ApplyDinnerSort(query, sort, descending);

            if(pageIndex.HasValue && pageSize.HasValue)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<List<Dinner>> GetPopularDinnersAsync()
        {
            return await _database.Dinners
                .Include(d => d.Rsvps)
                .OrderByDescending(d => d.Rsvps.Count)
                .Take(8)
                .ToListAsync();
        }

        public virtual async Task<Dinner> CreateDinnerAsync(Dinner dinner)
        {
            var rsvp = new Rsvp
            {
                UserName = dinner.UserName
            };

            dinner.Rsvps = new List<Rsvp> { rsvp };

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
                foreach (Rsvp rsvp in dinner.Rsvps)
                {
                    _database.Rsvp.Remove(rsvp);
                }

                _database.Dinners.Remove(dinner);

                await _database.SaveChangesAsync();
            }

            // Else no errors - this operation is idempotent
        }

        public virtual int GetDinnersCount()
        {
            return _database.Dinners.Where(d => d.EventDate >= DateTime.Now).Count();
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
