namespace Eventflow.Domain.Interfaces.Repositories {
   public interface IRoleRepository {
      public Task<Dictionary<int, string>> GetRoleIdToNameMapAsync();
   }
}