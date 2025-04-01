using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Models;
using Eventflow.Infrastructure.Data.Interfaces;

namespace Eventflow.Infrastructure.Repositories
{
    public class PersonalEventRepository : IPersonalEventRepository
    {
        private readonly IDbHelper _dbHelper;
        public PersonalEventRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task CreateEventAsync(PersonalEvent personalEvent)
        {
            string insertPersonalEventQuery = @"
                        INSERT INTO PersonalEvent (Title, Description, Date, UserId, IsCompleted, CategoryId)
                        VALUES (@Title, @Description, @Date, @UserId, @IsCompleted, @CategoryId);";

            var parameters = new Dictionary<string, object?>
            {
                { "@Title", personalEvent.Title },
                { "@Description", (object?)personalEvent.Description ?? DBNull.Value },
                { "@Date", personalEvent.Date },
                { "@UserId", personalEvent.UserId },
                { "@IsCompleted", personalEvent.IsCompleted },
                { "@CategoryId", (object?)personalEvent.CategoryId ?? DBNull.Value }
            };

            await _dbHelper.ExecuteNonQueryAsync(insertPersonalEventQuery, parameters!);
        }
    }
}
