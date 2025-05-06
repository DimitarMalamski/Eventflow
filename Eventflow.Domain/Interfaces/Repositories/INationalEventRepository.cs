using Eventflow.Domain.Models.Entities;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface INationalEventRepository
    {
        public Task<List<NationalEvent>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month);
    }
}
