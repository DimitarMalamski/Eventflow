namespace Eventflow.Application.Services.Interfaces
{
    public interface ICountryPopulationService
    {
        public Task PopulateCountriesAndContinentsAsync();
    }
}
