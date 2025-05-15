using Eventflow.Application.Security;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.Domain.Models.Entities;

namespace Eventflow.Application.Startup {
   public static class DbSeeder {
      public static async Task SeedAdminUserAsync(IUserRepository userRepository) {
         const string adminUsername = "admin";
         const string adminEmail = "admin@gmail.com";
         const string adminPassword = "Admin123!";
         const int adminRoleId = 1;

         var existingAdmin = await userRepository.GetByUsernameAsync(adminUsername);

         if (existingAdmin != null) return;

         var salt = PasswordHasher .GenerateRandomSalt();
         var hash = PasswordHasher.HashPassword(adminPassword, salt);

         var admin = new User
         {
            Username = adminUsername,
            Email = adminEmail,
            Firstname = "Admin",
            Lastname = "User",
            Salt = salt,
            PasswordHash = hash,
            RoleId = adminRoleId
         };

         await userRepository.RegisterUserAsync(admin);
         Console.WriteLine("✅ Admin user created.");
      }
   }
}