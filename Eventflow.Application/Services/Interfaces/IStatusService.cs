using Eventflow.Domain.Models.Common;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IStatusService
    {
        public Task<List<DropdownOption>> GetAllStatusOptionsAsync();
    }
}
