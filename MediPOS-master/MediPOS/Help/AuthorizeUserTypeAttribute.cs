using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace MediPOS.Help
{

    //[AuthorizeUserType(1,2)]  for all types authorization


    public class AuthorizeUserTypeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly int[] _allowedUserTypes;

        public AuthorizeUserTypeAttribute(params int[] allowedUserTypes)
        {
            _allowedUserTypes = allowedUserTypes;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;

            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectToActionResult("Login", "Login", null);
                return;
            }

            var userTypeClaim = user.Claims.FirstOrDefault(c => c.Type == "UserTypeId");

            if (userTypeClaim == null || !int.TryParse(userTypeClaim.Value, out int userTypeId))
            {
                context.Result = new RedirectToActionResult("UnAuth", "Login", null);
                return;
            }

            // Use the most specific (last declared) AuthorizeUserType on this endpoint
            var endpoint = context.ActionDescriptor.EndpointMetadata;
            var attributes = endpoint.OfType<AuthorizeUserTypeAttribute>().ToList();
            var mostSpecific = attributes.LastOrDefault();

            if (mostSpecific != null && !mostSpecific._allowedUserTypes.Contains(userTypeId))
            {
                context.Result = new RedirectToActionResult("UnAuth", "Login", null);
            }
        }
    }

}
