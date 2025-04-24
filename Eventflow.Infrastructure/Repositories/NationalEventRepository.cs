using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.ViewModels;
using Eventflow.Infrastructure.Data.Interfaces;
using Microsoft.Data.SqlClient;
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
        public async Task<List<NationalEventViewModel>> GetNationalHolidaysForCountryAsync(int countryId, int year, int month)
        {
            string query = @"
        SELECT ne.Title, ne.Description, ne.Date, ne.CountryId, c.Name AS CountryName
        FROM NationalEvent ne
        INNER JOIN Country c ON ne.CountryId = c.Id
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

                return new NationalEventViewModel
                {
                    Title = row["Title"].ToString()!,
                    Description = row["Description"]?.ToString() ?? "None",
                    Date = correctedDate, // 🔁 Correct year
                    CountryId = Convert.ToInt32(row["CountryId"]),
                    CountryName = row["CountryName"].ToString()!
                };
            }).ToList();
        }
    }
}
