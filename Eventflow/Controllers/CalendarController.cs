using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.ViewModels;
using Eventflow.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IContinentService _continentService;
        private readonly ICalendarService _calendarService;
        private readonly ICalendarNavigationService _calendarNavigationService;
        public CalendarController(IContinentService continentService,
            ICalendarService calendarService,
            ICalendarNavigationService calendarNavigationService)
        {
            _continentService = continentService;
            _calendarService = calendarService;
            _calendarNavigationService = calendarNavigationService;
        }
        public async Task<IActionResult> Index(int? month, int? year)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var model = new CalendarPageViewModel
            {
                Continents = await _continentService.OrderContinentByNameAsync(),
                Calendar = _calendarService.GenerateCalendar(normalizedYear, normalizedMonth),
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                }
            };

            return View(model);
        }
    }
}
