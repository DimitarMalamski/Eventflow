using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

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
        public async Task<IActionResult> LoadCalendarPartial(int? month, int? year)
        {
            int userId = GetUserId(HttpContext.Session);

            if (userId == 0)
            {
                return Unauthorized();
            }

            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var model = new CalendarComponentViewModel
            {
                Calendar = _calendarService.GenerateCalendar(normalizedYear, normalizedMonth),
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                }
            };

            return PartialView("~/Views/Shared/Partials/Calendar/_CalendarWrapper.cshtml", model);
        }
    }
}
