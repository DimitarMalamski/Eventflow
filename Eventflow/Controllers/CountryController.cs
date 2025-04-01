using Eventflow.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Eventflow.Controllers
{
    public class CountryController : Controller
    {
        private readonly ICountryService _countryService;
        public CountryController(ICountryService countryService)
        {
            _countryService = countryService;
        }

        [HttpGet]
        public async Task<JsonResult> GetCountriesByContinent(int continentId)
        {
            var countries = await _countryService.GetCountriesByContinentIdAsync(continentId);
            return Json(countries);
        }
    }
}
