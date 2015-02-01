using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    /// <summary>
    /// RSVP Controller class
    /// Performs CRUD operations
    /// </summary>
    [Route("api/[controller]")]
    public class RsvpController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="RsvpController"/> class.
        /// </summary>
        /// <param name="repository">nerd dinner repository</param>
        public RsvpController(INerdDinnerRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        /// <summary>
        /// Get  rsvp asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetRsvpsAsync(int dinnerId)
        {
            // TODO: Validate dinner owner is the user in Identity
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            return new JsonResult(dinner.Rsvps);
        }

        /// <summary>
        /// Create rsvp asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> CreateRsvpAsync(int dinnerId, int userId)
        {
            // TODO: Get user id from Identity
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            var rsvp = await _repository.CreateRsvpAsync(dinner, userId);
            return new JsonResult(rsvp);
        }

        /// <summary>
        /// Delete rsvp asynchronously
        /// </summary>
        /// <param name="dinnerId">dinner id</param>
        /// <param name="userId">user id</param>
        /// <returns></returns>
        [HttpDelete]
        public async Task<IActionResult> DeleteRsvpAsync(int dinnerId, int userId)
        {
            // TODO: Get user id from Identity
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            await _repository.DeleteRsvpAsync(dinner, userId);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}