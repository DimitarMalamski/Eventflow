using Eventflow.DTOs.DTOs;

namespace Eventflow.Application.Services.Interfaces
{
    public interface INationalEventService
    {
        public Task<List<NationalEventDto>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month);
    }
}
