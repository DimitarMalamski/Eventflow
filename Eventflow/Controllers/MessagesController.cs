using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class MessagesController : Controller
    {
        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Index()
        {
            ViewData["Title"] = "My messages";
            return View();
        }
    }
}
