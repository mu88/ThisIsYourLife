using System.Diagnostics.CodeAnalysis;
using BusinessServices.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BusinessServices;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static void AddBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<ILifePointService, LifePointService>();
        services.AddScoped<IPersonService, PersonService>();
    }
}