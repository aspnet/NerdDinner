using System;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RsvpController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        public RsvpController(INerdDinnerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous]
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

        [HttpPost]
        public async Task<IActionResult> CreateRsvpAsync(int dinnerId, string userName)
        {
            // TODO: Get user id from Identity
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            var rsvp = await _repository.CreateRsvpAsync(dinner, userName);
            return new JsonResult(rsvp);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRsvpAsync(int dinnerId, string userName)
        {
            // TODO: Get user id from Identity
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            await _repository.DeleteRsvpAsync(dinner, userName);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}