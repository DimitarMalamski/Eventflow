using Eventflow.Models.Models;
using Eventflow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IContinentService _continentService;
        private readonly ICalendarService _calendarService;
        public CalendarController(IContinentService continentService, ICalendarService calendarService)
        {
            _continentService = continentService;
            _calendarService = calendarService;
        }
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            List<Continent> continents = await _continentService.OrderContinentByNameAsync();

            ViewBag.CalendarHtml = await _calendarService.GenerateCalendarHtmlAsync(DateTime.Now.Year, DateTime.Now.Month);

            return View(continents);
        }
    }
}
