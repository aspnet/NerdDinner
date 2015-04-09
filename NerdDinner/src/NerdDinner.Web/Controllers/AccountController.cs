using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Mvc;
using NerdDinner.Web.Models;
using NerdDinner.Web.Persistence;

namespace NerdDinner.Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly INerdDinnerRepository _repository;

        public AccountController(INerdDinnerRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> Login([FromBody] LoginViewModel model)
        {
            var result = await _repository.SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (result.Succeeded)
            {
                return true;
            }

            return false;
        }

        [HttpPost]
        public void LogOff()
        {
            _repository.SignInManager.SignOut();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> Register([FromBody] RegisterViewModel model)
        {
            var user = new ApplicationUser { UserName = model.UserName };
            var result = await _repository.UserManager.CreateAsync(user, model.Password, cancellationToken: Context.RequestAborted);
            if (result.Succeeded)
            {
                await _repository.SignInManager.SignInAsync(user, isPersistent: false);
                return true;
            }

            return false;
        }

        [AllowAnonymous]
        public ActionResult ExternalLogin([FromQuery] ExternalLoginViewModel model)
        {
            // Request a redirect to the external login provider
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = model.ReturnUrl });
            var properties = _repository.SignInManager.ConfigureExternalAuthenticationProperties(model.Provider, redirectUrl);
            return new ChallengeResult(model.Provider, properties);
        }

        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var loginInfo = await _repository.SignInManager.GetExternalLoginInfoAsync(cancellationToken: Context.RequestAborted);
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            var result = await _repository.SignInManager.ExternalLoginSignInAsync(loginInfo.LoginProvider, loginInfo.ProviderKey, isPersistent: false, cancellationToken: Context.RequestAborted);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            else
            {
                var user = new ApplicationUser { UserName = loginInfo.ExternalIdentity.FindFirstValue(ClaimTypes.NameIdentifier) };
                var createResult = await _repository.UserManager.CreateAsync(user, cancellationToken: Context.RequestAborted);
                if (createResult.Succeeded)
                {
                    await _repository.SignInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToLocal(returnUrl);
                }
            }

            // error
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Manage");
            }

                // Get the information about the user from the external login provider
                var info = await _repository.SignInManager.GetExternalLoginInfoAsync(cancellationToken: Context.RequestAborted);
                if (info == null)
                {
                    return View("ExternalLoginFailure");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _repository.UserManager.CreateAsync(user, cancellationToken: Context.RequestAborted);

                if (result.Succeeded)
                {
                    result = await _repository.UserManager.AddLoginAsync(user, info, cancellationToken: Context.RequestAborted);
                    if (result.Succeeded)
                    {
                        await _repository.SignInManager.SignInAsync(user, isPersistent: false, cancellationToken: Context.RequestAborted);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);

            ViewBag.ReturnUrl = returnUrl;
            return View(model);
        }

        [AllowAnonymous]
        public ActionResult ExternalLoginFailure()
        {
            return View();
        }

        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private async Task<ApplicationUser> GetCurrentUserAsync()
        {
            return await _repository.UserManager.FindByIdAsync(Context.User.Identity.GetUserId(), cancellationToken: Context.RequestAborted);
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
    }
}