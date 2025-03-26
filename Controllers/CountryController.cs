using Eventflow.Services.Interfaces;
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
        public JsonResult GetCountriesByContinent(int continentId)
        {
            var countries = _countryService.GetCountriesByContinentId(continentId);
            return Json(countries);
        }
    }
}
