using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Common;

namespace Eventflow.Application.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository;
        }
        public async Task<List<DropdownOption>> GetAllStatusOptionsAsync()
            => await _statusRepository.GetAllStatusOptionsAsync();
    }
}
