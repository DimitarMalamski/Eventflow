using Eventflow.Domain.Models.Models;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IPersonalEventRepository
    {
        public Task CreateEventAsync(PersonalEvent personalEvent);
    }
}
