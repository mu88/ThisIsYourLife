using System.Diagnostics.CodeAnalysis;
using BusinessServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static void AddPersistence(this IServiceCollection services)
    {
        services.AddDbContext<Storage>(options => options.UseLazyLoadingProxies().UseSqlite($"Data Source=\"{Storage.DatabasePath}\""));
        services.AddScoped<IStorage>(provider => provider.GetRequiredService<Storage>());
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IImageService, ImageService>();

        services.AddHealthChecks().AddDbContextCheck<Storage>();
    }
}
