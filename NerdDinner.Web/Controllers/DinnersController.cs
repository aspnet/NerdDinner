using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;
using NerdDinner.Web.Common;

namespace NerdDinner.Web.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class DinnersController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        private readonly UserManager<ApplicationUser> _userManager;

        public DinnersController(INerdDinnerRepository repository, UserManager<ApplicationUser> userManager)
        {
            _repository = repository;
            _userManager = userManager;
        }

        [HttpGet("{id:int}", Name = "GetDinnerById")]
        [AllowAnonymous]
        public async Task<IActionResult> GetDinnerAsync(int id)
        {
            var dinner = await _repository.GetDinnerAsync(id);
            if (dinner == null)
            {
                return new NotFoundResult();
            }

            return new ObjectResult(dinner);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IEnumerable<Dinner>> GetDinnersAsync(
            DateTime? startDate,
            DateTime? endDate,
            double? lat,
            double? lng,
            int? pageIndex,
            int? pageSize,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            return await _repository.GetDinnersAsync(startDate, endDate, string.Empty, searchQuery, sort, descending, lat, lng, pageIndex, pageSize);
        }

        [HttpGet("my")]
        [AllowAnonymous]
        public async Task<IEnumerable<Dinner>> GetMyDinnersAsync(
            DateTime? startDate,
            DateTime? endDate,
            double? lat,
            double? lng,
            int? pageIndex,
            int? pageSize,
            string searchQuery = null,
            string sort = null,
            bool descending = false)
        {
            var user = await GetCurrentUserAsync();
            return await _repository.GetDinnersAsync(startDate, endDate, user.UserName, searchQuery, sort, descending, lat, lng, pageIndex, pageSize);
        }

        [HttpGet("popular")]
        [AllowAnonymous]
        public async Task<IEnumerable<Dinner>> GetPopularDinnersAsync()
        {
            return await _repository.GetPopularDinnersAsync();
        }

        [HttpGet("count")]
        [AllowAnonymous]
        public int GetDinnersCount()
        {
            return _repository.GetDinnersCount();
        }

        [HttpGet("isUserHost")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserHost(int id)
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return new ObjectResult(false);
            }

            var dinner = await _repository.GetDinnerAsync(id);
            return new ObjectResult(dinner.IsUserHost(user.UserName));
        }

        [HttpGet("isUserRegistered")]
        [AllowAnonymous]
        public async Task<IActionResult> IsUserRegistered(int id)
        {
            var user = await GetCurrentUserAsync();

            if (user == null)
            {
                return new ObjectResult(false);
            }

            var dinner = await _repository.GetDinnerAsync(id);
            return new ObjectResult(dinner.IsUserRegistered(user.UserName));
        }

        [HttpPost]
        public async Task<IActionResult> CreateDinnerAsync([FromBody] Dinner dinner)
        {
            var user = await GetCurrentUserAsync();
            dinner.UserName = user.UserName;

            GeoLocation.SearchByPlaceNameOrZip(dinner);
            dinner = await _repository.CreateDinnerAsync(dinner);
            var url = Url.RouteUrl("GetDinnerById", new { id = dinner.DinnerId }, Request.Scheme, Request.Host.ToUriComponent());

            HttpContext.Response.StatusCode = (int)HttpStatusCode.Created;
            HttpContext.Response.Headers["Location"] = url;
            return new ObjectResult(dinner);
        }

        [HttpPut("{id:int}", Name = "UpdateDinnerById")]
        public async Task<IActionResult> UpdateDinnerAsync(int id, [FromBody] Dinner dinner)
        {
            if (dinner.DinnerId != id)
            {
                return new BadRequestResult();
            }

            var user = await GetCurrentUserAsync();
            if (!dinner.IsUserHost(user.UserName))
            {
                return new NotFoundResult();
            }

            GeoLocation.SearchByPlaceNameOrZip(dinner);
            dinner = await _repository.UpdateDinnerAsync(dinner);
            return new ObjectResult(dinner);
        }

        [HttpDelete("{id:int}", Name = "DeleteDinnerById")]
        public async Task<IActionResult> DeleteDinnerAsync(int id)
        {
            var dinner = await _repository.GetDinnerAsync(id);
            var user = await GetCurrentUserAsync();

            if (!dinner.IsUserHost(user.UserName))
            {
                return new NotFoundResult();
            }

            await _repository.DeleteDinnerAsync(id);
            return new NoContentResult();
        }

        #region Helpers
        private Task<ApplicationUser> GetCurrentUserAsync()
        {
            return _userManager.GetUserAsync(HttpContext.User);
        }

        #endregion
    }
}
