using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;
using Eventflow.Infrastructure.Data.Interfaces;
using System.Data;

namespace Eventflow.Infrastructure.Repositories
{
    public class NationalEventRepository : INationalEventRepository
    {
        private readonly IDbHelper _dbHelper;
        public NationalEventRepository(IDbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public async Task<List<NationalEvent>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month)
        {
            string query = @"
                SELECT ne.Title, ne.Description, ne.Date, ne.CountryId
                FROM NationalEvent ne
                WHERE ne.CountryId = @CountryId
                AND MONTH(ne.Date) = @Month";

            var parameters = new Dictionary<string, object>
            {
                { "@CountryId", countryId },
                { "@Month", month }
            };

            var table = await _dbHelper.ExecuteQueryAsync(query, parameters);

            return table.Rows.Cast<DataRow>().Select(row =>
            {
                var originalDate = Convert.ToDateTime(row["Date"]);
                int safeDay = Math.Min(originalDate.Day, DateTime.DaysInMonth(year, month));
                var correctedDate = new DateTime(year, month, safeDay);

                return new NationalEvent
                {
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString() ?? "None",
                    Date = correctedDate,
                    CountryId = Convert.ToInt32(row["CountryId"]),
                };
            }).ToList();
        }
    }
}
