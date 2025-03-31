using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class EventController : Controller
    {
        public IActionResult MyEvents()
        {
            if (GetUserRoleId(HttpContext.Session) == 0)
            {
                TempData["Error"] = "You do not have access to this page.";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }
    }
}
