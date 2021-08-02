using BusinessServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessServices
{
    public static class Startup
    {
        public static void AddBusinessServices(this IServiceCollection services)
        {
            services.AddScoped<ILifePointService, LifePointService>();
        }
    }
}