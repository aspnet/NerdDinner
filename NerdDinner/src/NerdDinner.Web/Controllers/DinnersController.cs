using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DinnersController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        public DinnersController(INerdDinnerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("{id:int}", Name = "GetDinnerById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDinnerAsync(int id)
        {
            var dinner = await _repository.GetDinnerAsync(id);
            if (dinner == null)
            {
                return HttpNotFound();
            }

            return new JsonResult(dinner);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<Dinner>> GetDinnersAsync(
            DateTime? startDate,
            DateTime? endDate,
            bool myDinners = false,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            string userName = string.Empty;

            if (myDinners && !string.IsNullOrEmpty(Context.User.Identity.Name))
            {
                var user = await _repository.FindUserAsync(Context.User.Identity.Name);
                if (user != null)
                {
                    userName = user.UserName;
                }
            }

            return await _repository.GetDinnersAsync(startDate, endDate, userName, searchQuery, sort, descending);
        }

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IEnumerable<Dinner>> GetPopularDinnersAsync()
        {
            return await _repository.GetPopularDinnersAsync();
        }

        [HttpPost]
        public async Task<IActionResult> CreateDinnerAsync([FromBody] Dinner dinner)
        {
            var user = await _repository.FindUserAsync(Context.User.Identity.Name);
            dinner.UserName = user.UserName;

            // TODO: The user id in the dinner should be populated from the identity
            dinner = await _repository.CreateDinnerAsync(dinner);
            var url = Url.RouteUrl("GetDinnerById", new { id = dinner.DinnerId }, Request.Scheme, Request.Host.ToUriComponent());

            Context.Response.StatusCode = (int)HttpStatusCode.Created;
            Context.Response.Headers["Location"] = url;
            return new JsonResult(dinner);
        }

        [HttpPut("{id:int}", Name = "UpdateDinnerById")]
        public async Task<IActionResult> UpdateDinnerAsync(int id, Dinner dinner)
        {
            // TODO: Validate if updating dinner was created by user in identity
            if (dinner.DinnerId != id)
            {
                return new HttpStatusCodeResult((int)HttpStatusCode.BadRequest);
            }

            dinner = await _repository.UpdateDinnerAsync(dinner);
            return new JsonResult(dinner);
        }

        [HttpDelete("{id:int}", Name = "DeleteDinnerById")]
        public async Task<IActionResult> DeleteDinnerAsync(int id)
        {
            // TODO: Validate if deleting dinner was created by user in identity
            await _repository.DeleteDinnerAsync(id);
            return new HttpStatusCodeResult((int)HttpStatusCode.NoContent);
        }
    }
}
