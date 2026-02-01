using System.Diagnostics.CodeAnalysis;
using BusinessServices;
using Microsoft.AspNetCore.Components.Web;
using mu88.Shared.OpenTelemetry;
using Persistence;
using Serilog;
using WebApp.Services;
using WebApp.Shared;

#pragma warning disable CA1861

var builder = WebApplication.CreateBuilder(args);

builder.ConfigureOpenTelemetry("thisisyourlife");

// Configure logging and configuration
builder.Host.UseSerilog((context, services, configuration) => configuration
                                                              .ReadFrom.Configuration(context.Configuration)
                                                              .ReadFrom.Services(services)
                                                              .Enrich.FromLogContext()
                                                              .WriteTo.Console(
                                                                  outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.FFFK} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                                                              .WriteTo.File(Path.Combine("/home", "app", "data", "logs", "ThisIsYourLife.log"),
                                                                  rollingInterval: RollingInterval.Day,
                                                                  retainedFileCountLimit: 14));
builder.Configuration.AddJsonFile(Path.Combine("/home", "app", "data", "user.json"), true);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options =>
{
    options.RootComponents.RegisterCustomElement<LifePointDetail>("life-point-detail");
    options.RootComponents.RegisterCustomElement<NewLifePoint>("new-life-point");
    options.RootComponents.RegisterCustomElement<FilterLifePoints>("filter-life-points");
});

builder.Services.AddHealthChecks();
builder.Services.AddPersistence();
builder.Services.AddBusinessServices();
builder.Services.AddSingleton<INewLifePointDateService, NewLifePointDateService>();
builder.Services.AddAutoMapper(config => config.AddProfile<AutoMapperProfile>());
builder.Services.AddOptions<UserConfig>().Bind(builder.Configuration);
builder.Services.AddLocalization();

var app = builder.Build();

await CreateDbIfNotExistsAsync(app);

app.UsePathBase("/thisIsYourLife");

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}

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
app.MapHealthChecks("/healthz");

await app.RunAsync();

static async Task CreateDbIfNotExistsAsync(IHost host)
{
    await using var scope = host.Services.CreateAsyncScope();
    var services = scope.ServiceProvider;

    try
    {
        await services.GetRequiredService<IStorage>().EnsureStorageExistsAsync();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred creating the DB");
    }
}

[ExcludeFromCodeCoverage]
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1106:Code should not contain empty statements", Justification = "Necessary for code coverage")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "S1118", Justification = "Necessary for code coverage")]
public partial class Program;