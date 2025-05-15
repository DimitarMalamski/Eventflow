using Eventflow.Utilities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Eventflow.Attributes
{
    public class RequireUserOrAdmin : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = context.HttpContext.Session;
            var roleName = SessionHelper.GetUserRoleName(session);

            if (string.IsNullOrEmpty(roleName) || (roleName != "User" && roleName != "Admin"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            await next();
        }
    }
}
