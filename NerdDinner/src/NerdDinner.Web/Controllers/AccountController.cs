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
    /// <summary>
    /// Account Controller class
    /// Performs CRUD operations
    /// </summary>
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="repository">nerd dinner repository</param>
        public AccountController(INerdDinnerRepository repository)
        {
            if (repository == null)
            {
                throw new ArgumentNullException("repository");
            }

            _repository = repository;
        }

        /// <summary>
        /// Register user
        /// </summary>
        /// <param name="user">user</param>
        /// <returns></returns>
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

        /// <summary>
        /// Unsubscribe user
        /// </summary>
        /// <returns></returns>
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