using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Services.Interfaces
{
    public interface ICountryService
    {
        public Task<List<Country>> GetCountriesByContinentIdAsync(int continentId);
    }
}
