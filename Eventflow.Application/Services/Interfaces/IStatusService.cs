using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Application.Services.Interfaces
{
    public interface IStatusService
    {
        public Task<List<DropdownOption>> GetAllStatusOptionsAsync();
    }
}
