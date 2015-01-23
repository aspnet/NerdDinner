using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Common;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    /// <summary>
    /// Dinners Controller class
    /// Performs CRUD operations
    /// </summary>
    [Route("api/[controller]")]
    public class DinnersController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="DinnersController"/> class.
        /// </summary>
        /// <param name="repository">nerd dinner repository</param>
        public DinnersController(INerdDinnerRepository repository)
        {
            repository.EnsureArgumentNotNull("repository");
            _repository = repository;
        }

        /// <summary>
        /// Gets dinner which user requests
        /// </summary>
        /// <param name="id">dinner id is sent</param>
        /// <returns>return dinner if dinner exists else a 404</returns>
        [HttpGet("{id:long}", Name = "GetDinnerById")]
        public async Task<IActionResult> GetDinnerAsync(long id)
        {
            var dinner = await _repository.GetDinnerAsync(id);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            return new JsonResult(dinner);
        }

        /// <summary>
        /// Gets all dinners for given filter options
        /// </summary>
        /// <param name="startDate">start date of event</param>
        /// <param name="endDate">end date of event</param>
        /// <param name="userId">hosted by user id</param>
        /// <param name="searchQuery">search text</param>
        /// <param name="sort">sort parameter</param>
        /// <param name="descending">sort order</param>
        /// <returns>List of all dinners</returns>
        [HttpGet]
        public async Task<IEnumerable<Dinner>> GetDinnersAsync(
            DateTime? startDate,
            DateTime? endDate,
            long userId = 0,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            var query = _repository.Dinners.AsQueryable();

            if (userId != 0)
            {
                query = query.Where(d => d.HostedByUserId == userId);
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
                query = query.Where(d => d.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) || d.Description.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(sort))
            {
                query = query.OrderByPropertyName(sort, descending);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        /// Updates the changes made in the dinner
        /// </summary>
        /// <param name="id">dinner id is sent</param>
        /// <param name="dinner">Updated Dinner Object</param>
        /// <returns>updated dinner if success else 400</returns>
        [HttpPut]
        public async Task<IActionResult> UpdateDinnerAsync(long id, Dinner dinner)
        {
            if (dinner.DinnerId != id)
            {
                // return bad request
            }

            dinner = await _repository.UpdateDinnerAsync(id, dinner);
            if (dinner != null)
            {
                return new JsonResult(dinner);
            }
            else
            {
                return HttpNotFound();
            }
        }

        /// <summary>
        /// Adds the changes made in the dinner
        /// </summary>
        /// <param name="dinner">Initial dinner object</param>
        /// <returns>added dinner if success else 400</returns>
        [HttpPost]
        public async Task<IActionResult> AddDinnerAsync(Dinner dinner)
        {
            dinner = await _repository.AddDinnerAsync(dinner);
            string url = Url.RouteUrl("GetDinnerById", new { id = dinner.DinnerId }, Request.Scheme, Request.Host.ToUriComponent());

            Context.Response.StatusCode = (int)HttpStatusCode.Created;
            Context.Response.Headers["Location"] = url;
            return new JsonResult(dinner);
        }

        /// <summary>
        /// Deletes the dinner with given id
        /// </summary>
        /// <param name="id">dinner id</param>
        /// <returns>Return 204 if success else 404</returns>
        [HttpDelete("{id:long}", Name = "DeleteDinnerById")]
        public async Task<IActionResult> DeleteDinnerAsync(long id)
        {
            await _repository.DeleteDinnerAsync(id);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}
