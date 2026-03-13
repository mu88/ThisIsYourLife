using System.Diagnostics.CodeAnalysis;
using BusinessServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Persistence;

[ExcludeFromCodeCoverage]
public static class Startup
{
    public static void AddPersistence(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.AddDbContext<Storage>((sp, options) =>
        {
            var storageOptions = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
            var dbPath = Path.Combine(storageOptions.BasePath, "db", "ThisIsYourLife.db");
            options.UseLazyLoadingProxies().UseSqlite($"Data Source=\"{dbPath}\"");
        });
        services.AddScoped<IStorage>(provider => provider.GetRequiredService<Storage>());
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IImageService, ImageService>();

        services.AddHealthChecks().AddDbContextCheck<Storage>();
    }
}
