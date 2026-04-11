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
    public static void AddPersistence(this IServiceCollection services, IConfigurationManager configuration)
    {
        var storageOptions = configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
        configuration.AddJsonFile(Path.Combine(storageOptions.BasePath, "user.json"), optional: true);

        services.Configure<StorageOptions>(configuration.GetSection(StorageOptions.SectionName));
        services.AddOptions<UserConfig>().Bind(configuration);
        services.AddDbContext<Storage>((sp, options) =>
        {
            var opts = sp.GetRequiredService<IOptions<StorageOptions>>().Value;
            options.UseLazyLoadingProxies().UseSqlite($"Data Source=\"{opts.DatabasePath}\"");
        });
        services.AddScoped<IStorage>(provider => provider.GetRequiredService<Storage>());
        services.AddSingleton<IFileSystem, FileSystem>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IImageService, ImageService>();

        services.AddHealthChecks().AddDbContextCheck<Storage>();
    }
}
