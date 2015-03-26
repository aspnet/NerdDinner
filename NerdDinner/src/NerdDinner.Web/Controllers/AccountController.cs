using System;
using System.Net;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        public AccountController(INerdDinnerRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUserAsync(User user)
        {
            IdentityResult result = await _repository.RegisterUserAsync(user);

            if (result == null)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            else if (!result.Succeeded)
            {
                // throw bad request
                return new JsonResult(result.Errors);
            }
            else
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
        }

        [HttpPost]
        [Route("unsubscribe")]
        public async Task<IActionResult> UnsubscribeUserAsync()
        {
            var userName = Context.User.Identity.GetUserName();
            IdentityResult result = await _repository.UnsubscribeUserAsync(userName);

            if (result == null)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.InternalServerError);
            }
            else if (!result.Succeeded)
            {
                Context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JsonResult(result.Errors);
            }
            else
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.OK);
            }
        }
    }
}