using Eventflow.Application.Services.Interfaces;
using Eventflow.Attributes;
using Eventflow.Domain.Models.Models;
using Eventflow.Domain.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using static Eventflow.Utilities.SessionHelper;

namespace Eventflow.Controllers
{
    public class EventController : Controller
    {
        private readonly ICalendarNavigationService _calendarNavigationService;
        private readonly ICalendarService _calendarService;
        private readonly IContinentService _continentService;
        private readonly ICategoryService _categoryService;
        private readonly IPersonalEventService _personalEventService;
        public EventController(ICalendarNavigationService calendarNavigationService,
            ICalendarService calendarService,
            IContinentService continentService,
            ICategoryService categoryService,
            IPersonalEventService personalEventService
            )
        {
            _calendarNavigationService = calendarNavigationService;
            _calendarService = calendarService;
            _continentService = continentService;
            _categoryService = categoryService;
            _personalEventService = personalEventService;
        }

        [RequireUserOrAdmin]
        public async Task<IActionResult> MyEvents(int? year, int? month)
        {
            if (GetUserRoleId(HttpContext.Session) == 0)
            {
                TempData["Error"] = "You do not have access to this page.";
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

        [HttpGet]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create()
        {
            var model = new CreatePersonalEventViewModel
            {
                Date = DateTime.Now,
                Category = await _categoryService.GetAllCategoriesAsync()
            };

            return View(model);
        }

        [HttpPost]
        [RequireUserOrAdmin]
        public async Task<IActionResult> Create(CreatePersonalEventViewModel model)
        {

            if (!ModelState.IsValid)
            {
                model.Category = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            int userId = GetUserId(HttpContext.Session);

            var newEvent = new PersonalEvent
            {
                Title = model.Title,
                Description = model.Description,
                Date = model.Date,
                CategoryId = model.CategoryId,
                UserId = userId,
                IsCompleted = false
            };

            await _personalEventService.CreateAsync(newEvent);

            return RedirectToAction("MyEvents");
        }
    }
}
