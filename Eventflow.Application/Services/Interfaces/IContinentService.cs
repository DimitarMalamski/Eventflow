using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IContinentService
    {
        public Task<List<Continent>> OrderContinentByNameAsync();
    }
}
