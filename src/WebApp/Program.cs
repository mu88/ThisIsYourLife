using System;
using System.IO;
using System.Threading.Tasks;
using BusinessServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence;
using Serilog;
using WebApp.Services;
using WebApp.Shared;

var builder = WebApplication.CreateBuilder(args);

// Configure logging and configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
                            .ReadFrom.Configuration(context.Configuration)
                            .ReadFrom.Services(services)
                            .Enrich.FromLogContext()
                            .WriteTo.Console()
                            .WriteTo.File(Path.Combine("data", "logs", "ThisIsYourLife.log"), rollingInterval: RollingInterval.Day, retainedFileCountLimit: 14));
builder.Configuration.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", "user.json"), true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.RootComponents.RegisterAsCustomElement<LifePointDetail>("life-point-detail");
    options.RootComponents.RegisterAsCustomElement<NewLifePoint>("new-life-point");
    options.RootComponents.RegisterAsCustomElement<FilterLifePoints>("filter-life-points");
});

builder.Services.AddPersistence();
builder.Services.AddBusinessServices();
builder.Services.AddSingleton<INewLifePointDateService, NewLifePointDateService>();
builder.Services.AddAutoMapper(config => config.AddProfile(typeof(AutoMapperProfile)));
builder.Services.AddOptions<UserConfig>().Bind(builder.Configuration);
builder.Services.AddLocalization();

var app = builder.Build();

await CreateDbIfNotExistsAsync(app);

app.UsePathBase("/thisIsYourLife");

if (!app.Environment.IsDevelopment()) app.UseExceptionHandler("/Error");

app.UseStaticFiles();
app.UseRouting();

app.UseRequestLocalization(localizationOptions =>
{
    var supportedCultures = new[] { "en", "de" };
    localizationOptions.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");
app.MapControllers();

app.Run();

static async Task CreateDbIfNotExistsAsync(IHost host)
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