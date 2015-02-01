using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
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
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        /// <summary>
        /// Gets dinner which user requests
        /// </summary>
        /// <param name="id">dinner id is sent</param>
        /// <returns>return dinner if dinner exists else a 404</returns>
        [HttpGet("{id:int}", Name = "GetDinnerById")]
        public async Task<IActionResult> GetDinnerAsync(int id)
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
            int userId = 0,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            return await _repository.GetDinnersAsync(startDate, endDate, userId, searchQuery, sort, descending);
        }

        /// <summary>
        /// Adds the changes made in the dinner
        /// </summary>
        /// <param name="dinner">Initial dinner object</param>
        /// <returns>added dinner if success else 400</returns>
        [HttpPost]
        public async Task<IActionResult> CreateDinnerAsync(Dinner dinner)
        {
            //TODO: The user id in the dinner should be populated from the identity
            dinner = await _repository.CreateDinnerAsync(dinner);
            var url = Url.RouteUrl("GetDinnerById", new { id = dinner.DinnerId }, Request.Scheme, Request.Host.ToUriComponent());

            Context.Response.StatusCode = (int)HttpStatusCode.Created;
            Context.Response.Headers["Location"] = url;
            return new JsonResult(dinner);
        }

        /// <summary>
        /// Updates the changes made in the dinner
        /// </summary>
        /// <param name="id">dinner id is sent</param>
        /// <param name="dinner">Updated Dinner Object</param>
        /// <returns>updated dinner if success else 400</returns>
        [HttpPut("{id:int}", Name = "UpdateDinnerById")]
        public async Task<IActionResult> UpdateDinnerAsync(int id, Dinner dinner)
        {
            //TODO: Validate if updating dinner was created by user in identity
            if (dinner.DinnerId != id)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            dinner = await _repository.UpdateDinnerAsync(dinner);
            return new JsonResult(dinner);
        }

        /// <summary>
        /// Deletes the dinner with given id
        /// </summary>
        /// <param name="id">dinner id</param>
        /// <returns>Return 204 if success else 404</returns>
        [HttpDelete("{id:int}", Name = "DeleteDinnerById")]
        public async Task<IActionResult> DeleteDinnerAsync(int id)
        {
            //TODO: Validate if deleting dinner was created by user in identity
            await _repository.DeleteDinnerAsync(id);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}
