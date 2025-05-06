using Eventflow.Domain.Models.DTOs;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface INationalEventRepository
    {
        public Task<List<NationalEventDto>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month);
    }
}
