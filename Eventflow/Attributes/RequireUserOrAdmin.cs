using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Eventflow.Attributes
{
    public class RequireUserOrAdmin : Attribute, IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var session = context.HttpContext.Session;
            var role = session.GetString("Role");

            if (string.IsNullOrEmpty(role) || (role != "User" && role != "Admin"))
            {
                context.Result = new RedirectToActionResult("Login", "Account", null);
                return;
            }

            await next();
        }
    }
}
