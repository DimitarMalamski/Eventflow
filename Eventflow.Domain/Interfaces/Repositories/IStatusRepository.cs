using Eventflow.Domain.Models.Common;

namespace Eventflow.Domain.Interfaces.Repositories
{
    public interface IStatusRepository
    {
        public Task<List<DropdownOption>> GetAllStatusOptionsAsync();
    }
}
