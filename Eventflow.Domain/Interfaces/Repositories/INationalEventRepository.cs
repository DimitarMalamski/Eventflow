using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface INationalEventRepository
    {
        public Task<List<NationalEventViewModel>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month);
    }
}
