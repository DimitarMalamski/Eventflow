using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IContinentService
    {
        public Task<List<Continent>> OrderContinentByNameAsync();
    }
}
