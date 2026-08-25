using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using CoConnect.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CoConnect.Persistence;
using CoConnect.Persistence.Entities;
using CoConnect.Persistence.Specifications;

namespace CoConnect.Controllers
{
    public class AccountController : Controller
    {
        private readonly IDataContextFactory _dataContextFactory;

        public AccountController(IDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(new LoginViewModel
            {
                ReturnUrl = returnUrl
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using var dataContext = _dataContextFactory.CreateDataContext();
            var user = await dataContext.FindSingleAsync(UserSpecs.GetByUsername(model.Username));
            if (user == null || user.IsDisabled || user.PasswordHash != HashPassword(model.Password))
            {
                ModelState.AddModelError(string.Empty, "Invalid username or password.");
                return View(model);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim("security_stamp", user.SecurityStamp)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrWhiteSpace(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            {
                return Redirect(model.ReturnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }

            using var dataContext = _dataContextFactory.CreateDataContext();
            var user = await dataContext.FindSingleAsync(UserSpecs.Get(userId));
            if (user == null || user.IsDisabled)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return RedirectToAction(nameof(Login));
            }

            if (user.PasswordHash != HashPassword(model.CurrentPassword))
            {
                ModelState.AddModelError(nameof(ChangePasswordViewModel.CurrentPassword), "Current password is incorrect.");
                return View(model);
            }

            user.PasswordHash = HashPassword(model.NewPassword);
            user.SecurityStamp = Guid.NewGuid().ToString("N");
            user.UpdatedUtc = DateTimeOffset.UtcNow;

            dataContext.Update(user);
            await dataContext.SaveChangesAsync();

            TempData["ChangePasswordSuccess"] = "Password updated successfully.";

            return RedirectToAction(nameof(ChangePassword));
        }

        private static string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
