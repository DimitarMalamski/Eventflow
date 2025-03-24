using Eventflow.Data;
using Eventflow.Models.Models;
using Eventflow.Repositories.Interfaces;
using System.Data;

namespace Eventflow.Repositories
{
    public class ContinentRepository : IContinentRepository
    {
        private readonly DbHelper _dbHelper;
        public ContinentRepository(DbHelper dbHelper)
        {
            _dbHelper = dbHelper;
        }
        public List<Continent> GetAllContinents()
        {
            List<Continent> continents = new List<Continent>();

            string getAllContinenets = "SELECT Id, Name FROM [Continent]";

            DataTable dt = _dbHelper.ExecuteQuery(getAllContinenets);

            foreach (DataRow row in dt.Rows)
            {
                continents.Add(new Continent
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Name = row["Name"].ToString()!
                });
            }

            return continents;
        }
    }
}
