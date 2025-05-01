using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Exceptions;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Common;
using static Eventflow.Domain.Common.CustomErrorMessages.StatusService;

namespace Eventflow.Application.Services
{
    public class StatusService : IStatusService
    {
        private readonly IStatusRepository _statusRepository;
        public StatusService(IStatusRepository statusRepository)
        {
            _statusRepository = statusRepository
                ?? throw new ArgumentNullException(nameof(statusRepository));
        }
        public async Task<List<DropdownOption>> GetAllStatusOptionsAsync()
        {
            try
            {
                return await _statusRepository.GetAllStatusOptionsAsync();
            }
            catch (Exception)
            {
                throw new StatusRetrievalException(statusRetrievalFailed);
            }
        }
    }
}
