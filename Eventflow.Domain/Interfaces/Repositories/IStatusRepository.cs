using Eventflow.Domain.Models.ViewModels;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IStatusRepository
    {
        public Task<List<DropdownOption>> GetAllStatusOptionsAsync();
    }
}
