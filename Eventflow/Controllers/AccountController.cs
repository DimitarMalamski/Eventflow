using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.Entities;
using Eventflow.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;
        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User? user = await _authService.LoginAsync(model.LoginInput, model.Password);
            
            if (user != null)
            {
                SetUserSession(HttpContext.Session, user.Id, user.Username, user.RoleId);

                return RedirectToAction("Index", "Calendar");
            }

            ModelState.AddModelError("Password", "Invalid username or password.");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (await _authService.RegisterAsync(model.Username, model.Password, model.Firstname, model.Lastname, model.Email))
            {
                return RedirectToAction("Login");
            }

            ModelState.AddModelError("", "Registration failed. Username or email may already be taken.");
            return View(model);
        }

        [HttpGet]
        public IActionResult GuestLogin()
        {
            SetUserSession(HttpContext.Session, 0, "Guest", 0);

            return RedirectToAction("Index", "Calendar");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            ClearUserSession(HttpContext.Session);
            return RedirectToAction("Index", "Home");
        }
    }
}
