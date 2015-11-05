using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Authorization;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class RsvpController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        private readonly UserManager<ApplicationUser> _userManager;

        public RsvpController(INerdDinnerRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpPost]
        public async Task<IActionResult> CreateRsvpAsync(int dinnerId)
        {
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());
            var rsvp = await _repository.CreateRsvpAsync(dinner, user.UserName);
            return new JsonResult(rsvp);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRsvpAsync(int dinnerId)
        {
            var dinner = await _repository.GetDinnerAsync(dinnerId);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            var user = await _userManager.FindByIdAsync(Context.User.GetUserId());

            await _repository.DeleteRsvpAsync(dinner, user.UserName);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}