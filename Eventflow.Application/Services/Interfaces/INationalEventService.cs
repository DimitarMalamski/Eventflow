using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface INationalEventService
    {
        public Task<List<NationalEventViewModel>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month);
    }
}
