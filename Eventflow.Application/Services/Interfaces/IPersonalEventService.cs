using Eventflow.Domain.Models.Models;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IPersonalEventService
    {
        public Task CreateAsync(PersonalEvent personalEvent);
    }
}
