using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.ViewModels;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class StatusRepository : IStatusRepository
    {
        private readonly IDbHelper _dbHelper;
        public StatusRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<List<DropdownOption>> GetAllStatusOptionsAsync()
        {
            string getAllStatusesQuery = "SELECT * FROM Status ORDER BY Id";

            var dt = await _dbHelper.ExecuteQueryAsync(getAllStatusesQuery);

            return dt.AsEnumerable()
                .Select(row => new DropdownOption
                {
                    Id = row["Id"].ToString()!,
                    Name = row["Name"].ToString()!
                })
                .ToList();

            
        }
    }
}
