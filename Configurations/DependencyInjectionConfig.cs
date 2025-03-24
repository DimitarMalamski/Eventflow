using Eventflow.Data;
using Eventflow.Repositories.Interfaces;
using Eventflow.Repositories;
using Eventflow.Services.Interfaces;
using Eventflow.Services;

namespace Eventflow.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IContinentRepository, ContinentRepository>();

            // Services
            services.AddScoped<IAuthService, UserService>();
            services.AddScoped<IContinentService, ContinentService>();
            services.AddScoped<ICalendarService, CalendarService>();

            // DB Helper
            services.AddSingleton<DbHelper>();
        }
    }
}
