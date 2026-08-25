using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CoConnect.Persistence;
using CoConnect.Persistence.Specifications;

namespace CoConnect.Infrastructure.Auth
{
    public class CookiePrincipalValidator
    {
        private readonly IDataContextFactory _dataContextFactory;

        public CookiePrincipalValidator(IDataContextFactory dataContextFactory)
        {
            _dataContextFactory = dataContextFactory;
        }

        public async Task ValidateAsync(CookieValidatePrincipalContext context)
        {
            var principal = context.Principal;
            if (principal?.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var stamp = principal.FindFirstValue("security_stamp");
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(stamp))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }

            using var dataContext = _dataContextFactory.CreateDataContext();
            var user = await dataContext.FindSingleAsync(UserSpecs.Get(userId));
            if (user == null || user.IsDisabled || !string.Equals(user.SecurityStamp, stamp, StringComparison.Ordinal))
            {
                context.RejectPrincipal();
                await context.HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return;
            }
        }
    }
}
