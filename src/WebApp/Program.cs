using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
using BusinessServices;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Serilog;

namespace WebApp;

[ExcludeFromCodeCoverage]
public class Program
{
    public static async Task Main(string[] args)
    {
        var host = CreateHostBuilder(args)
            .UseSerilog((context, services, configuration) => configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .ReadFrom.Services(services)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.File(Path.Combine("logs", "ThisIsMyLife.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14))
            .Build();

        await CreateDbIfNotExistsAsync(host);

        await host.RunAsync();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(builder => builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "user.json"), true))
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

    private static async Task CreateDbIfNotExistsAsync(IHost host)
    {
        using var scope = host.Services.CreateScope();
        var services = scope.ServiceProvider;

        try { await services.GetRequiredService<IStorage>().EnsureStorageExistsAsync(); }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "An error occurred creating the DB");
        }
    }
}