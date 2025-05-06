using Eventflow.Application.Services.Interfaces;
using Eventflow.ViewModels.Calendar;
using Eventflow.ViewModels.Calendar.Component;
using Eventflow.ViewModels.Calendar.Page;
using Eventflow.ViewModels.Continent;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Controllers
{
    public class CalendarController : Controller
    {
        private readonly IContinentService _continentService;
        private readonly ICalendarService _calendarService;
        private readonly ICalendarNavigationService _calendarNavigationService;
        private readonly IPersonalEventService _personalEventService;
        public CalendarController(IContinentService continentService,
            ICalendarService calendarService,
            ICalendarNavigationService calendarNavigationService,
            IPersonalEventService personalEventService)
        {
            _continentService = continentService;
            _calendarService = calendarService;
            _calendarNavigationService = calendarNavigationService;
            _personalEventService = personalEventService;
        }
        public async Task<IActionResult> Index(int? month, int? year, int? countryId)
        {
            if (HttpContext.Session.GetString("UserId") == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var calendarDto = _calendarService.GenerateCalendar(normalizedYear, normalizedMonth);

            var calendarViewModel = new CalendarViewModel
            {
                Year = calendarDto.Year,
                Month = calendarDto.Month,
                Days = calendarDto.Days.Select(day => new CalendarDayViewModel
                {
                    DayNumber = day.DayNumber,
                    IsToday = day.IsToday,
                    Date = day.Date,
                    PersonalEvents = day.PersonalEvents.Select(e => new PersonalEventWithCategoryNameViewModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Date = e.Date,
                        IsCompleted = e.IsCompleted,
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        UserId = e.UserId,
                        IsInvited = e.IsInvited,
                        CreatorUsername = e.CreatorUsername,
                        IsCreator = e.IsCreator,
                        ParticipantUsernames = e.ParticipantUsernames
                    }).ToList(),
                    NationalEvents = day.NationalEvents.Select(n => new NationalEventViewModel
                    {
                        Title = n.Title,
                        Description = n.Description,
                        Date = n.Date,
                        CountryId = n.CountryId,
                        CountryName = n.CountryName
                    }).ToList()
                }).ToList()
            };

            var model = new CalendarPageViewModel
            {
                Continents = (await _continentService.OrderContinentByNameAsync())
                    .Select(c => new ContinentViewModel
                    {
                        Id = c.Id,
                        Name = c.Name
                    })
                    .ToList(),
                Calendar = calendarViewModel,
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                },
                SelectedCountryId = countryId
            };

            return View(model);
        }
        public async Task<IActionResult> LoadCalendarByCountryPartial(int countryId, int? month, int? year)
        {
            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var calendarDto = await _calendarService.GenerateNationalHolidayCalendarAsync(
                    countryId,
                    normalizedYear,
                    normalizedMonth);

            var calendarViewModel = new CalendarViewModel
            {
                Year = calendarDto.Year,
                Month = calendarDto.Month,
                Days = calendarDto.Days.Select(day => new CalendarDayViewModel
                {
                    DayNumber = day.DayNumber,
                    IsToday = day.IsToday,
                    Date = day.Date,
                    PersonalEvents = day.PersonalEvents.Select(e => new PersonalEventWithCategoryNameViewModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Date = e.Date,
                        IsCompleted = e.IsCompleted,
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        UserId = e.UserId,
                        IsInvited = e.IsInvited,
                        CreatorUsername = e.CreatorUsername,
                        IsCreator = e.IsCreator,
                        ParticipantUsernames = e.ParticipantUsernames
                    }).ToList(),
                    NationalEvents = day.NationalEvents.Select(n => new NationalEventViewModel
                    {
                        Title = n.Title,
                        Description = n.Description,
                        Date = n.Date,
                        CountryId = n.CountryId,
                        CountryName = n.CountryName
                    }).ToList()
                }).ToList()
            };

            var model = new CalendarComponentViewModel
            {
                Calendar = calendarViewModel,
                Navigation = new CalendarNavigationViewModel
                {
                    CurrentMonth = normalizedMonth,
                    CurrentYear = normalizedYear
                }
            };

            return PartialView("~/Views/Shared/Partials/Calendar/_CalendarWrapper.cshtml", model);
        }
        public IActionResult LoadEmptyCalendarPartial(int? month, int? year)
        {
            var (normalizedYear, normalizedMonth) = _calendarNavigationService.Normalize(year, month);

            var calendarDto = _calendarService.GenerateEmptyCalendar(normalizedYear, normalizedMonth);

            var calendarViewModel = new CalendarViewModel
            {
                Year = calendarDto.Year,
                Month = calendarDto.Month,
                Days = calendarDto.Days.Select(day => new CalendarDayViewModel
                {
                    DayNumber = day.DayNumber,
                    IsToday = day.IsToday,
                    Date = day.Date,
                    PersonalEvents = day.PersonalEvents.Select(e => new PersonalEventWithCategoryNameViewModel
                    {
                        Id = e.Id,
                        Title = e.Title,
                        Description = e.Description,
                        Date = e.Date,
                        IsCompleted = e.IsCompleted,
                        CategoryId = e.CategoryId,
                        CategoryName = e.CategoryName,
                        UserId = e.UserId,
                        IsInvited = e.IsInvited,
                        CreatorUsername = e.CreatorUsername,
                        IsCreator = e.IsCreator,
                        ParticipantUsernames = e.ParticipantUsernames
                    }).ToList(),
                    NationalEvents = day.NationalEvents.Select(n => new NationalEventViewModel
                    {
                        Title = n.Title,
                        Description = n.Description,
                        Date = n.Date,
                        CountryId = n.CountryId,
                        CountryName = n.CountryName
                    }).ToList()
                }).ToList()
            };

            var model = new CalendarComponentViewModel
            {
                Calendar = calendarViewModel,
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
