using Eventflow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Eventflow.Models.Models;
using Eventflow.Utilities;

namespace Eventflow.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string loginInput, string password)
        {
            User? user = _userService.Login(loginInput, password);
            
            if (user != null)
            {
                SessionHelper.SetUserSession(HttpContext.Session, user.Id, user.Username, user.RoleId);

                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string username, string password, string firstname, string? lastname, string email)
        {
            if (_userService.Register(username, password, firstname, lastname ?? null, email))
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Username or email may already be taken.");
            return View();
        }

        [HttpGet]
        public IActionResult GuestLogin()
        {
            SessionHelper.SetUserSession(HttpContext.Session, 0, "Guest", 0);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Logout()
        {
            SessionHelper.ClearUserSession(HttpContext.Session);
            return RedirectToAction("Login");
        }
    }
}
