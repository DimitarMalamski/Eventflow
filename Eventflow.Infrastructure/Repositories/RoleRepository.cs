using System.Data;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Infrastructure.Data.Interfaces;

namespace Eventflow.Infrastructure.Repositories {
   public class RoleRepository : IRoleRepository
   {
      private readonly IDbHelper _dbHelper;
      public RoleRepository(IDbHelper dbHelper)
      {
         _dbHelper = dbHelper;
      }
      public async Task<Dictionary<int, string>> GetRoleIdToNameMapAsync()
      {
         string getRoleToNameQuery = "SELECT Id, Name FROM [Role]";

         var dt = await _dbHelper.ExecuteQueryAsync(getRoleToNameQuery);

         return dt.AsEnumerable()
               .ToDictionary(row => Convert.ToInt32(row["Id"]),
                                 row => row["Name"].ToString()!);
      }
   }
}