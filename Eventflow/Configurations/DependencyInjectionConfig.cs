using Eventflow.Application.Services;
using Eventflow.Application.Services.Interfaces;
using Eventflow.Domain.Interfaces.Repositories;
using Eventflow.infrastructure.Repositories;
using Eventflow.Infrastructure.Data;
using Eventflow.Infrastructure.Data.Interfaces;
using Eventflow.Infrastructure.Repositories;

namespace Eventflow.Configurations
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IContinentRepository, ContinentRepository>();
            services.AddScoped<ICountryRepository, CountryRepository>();
            services.AddScoped<IPersonalEventRepository, PersonalEventRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IInviteRepository, InviteRepository>();

            // Services
            services.AddScoped<IAuthService, UserService>();
            services.AddScoped<IContinentService, ContinentService>();
            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ICountryService, CountryService>();
            services.AddScoped<ICountryPopulationService, CountryPopulationService>();
            services.AddScoped<ICalendarNavigationService, CalendarNavigationService>();
            services.AddScoped<IPersonalEventService, PersonalEventService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IInviteService, InviteService>();

            // DB Helper
            services.AddScoped<IDbHelper, DbHelper>();
        }
    }
}
