using System.Diagnostics.CodeAnalysis;
using BusinessServices;
using Microsoft.AspNetCore.Components.Web;
using mu88.Shared.OpenTelemetry;
using OpenTelemetry.Trace;
using Persistence;
using WebApp;
using WebApp.Services;
using WebApp.Shared;

#pragma warning disable CA1861

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureOpenTelemetry("thisisyourlife", builder.Configuration);
builder.Services.AddOpenTelemetry().WithTracing(tracing => tracing.AddSource(Logging.Extensions.Tracing.Source.Name));

// Configure logging and configuration
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.TimestampFormat = "yyyy-MM-dd HH:mm:ss.FFFK ";
    options.SingleLine = true;
});
var storageOptions = builder.Configuration.GetSection(StorageOptions.SectionName).Get<StorageOptions>() ?? new StorageOptions();
builder.Configuration.AddJsonFile(Path.Combine(storageOptions.BasePath, "user.json"), true);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.RootComponents.RegisterCustomElement<LifePointDetail>("life-point-detail");
        options.RootComponents.RegisterCustomElement<NewLifePoint>("new-life-point");
        options.RootComponents.RegisterCustomElement<FilterLifePoints>("filter-life-points");
    });

builder.Services.AddHealthChecks();
builder.Services.AddPersistence(builder.Configuration);
builder.Services.AddBusinessServices();
builder.Services.AddSingleton<INewLifePointDateService, NewLifePointDateService>();
builder.Services.AddAutoMapper(config => config.AddProfile<AutoMapperProfile>());
builder.Services.AddOptions<UserConfig>().Bind(builder.Configuration);
builder.Services.AddLocalization();

var app = builder.Build();

await CreateDbIfNotExistsAsync(app);

app.UsePathBase("/thisIsYourLife");
app.UseRouting();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.UseRequestLocalization(localizationOptions =>
{
    var supportedCultures = new[] { "en", "de" };
    localizationOptions.SetDefaultCulture(supportedCultures[0])
        .AddSupportedCultures(supportedCultures)
        .AddSupportedUICultures(supportedCultures);
});

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();
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

#pragma warning disable ASP0027
[ExcludeFromCodeCoverage]
[SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1106:Code should not contain empty statements", Justification = "Necessary for code coverage")]
[SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "S1118", Justification = "Necessary for code coverage")]
public partial class Program;
#pragma warning restore ASP0027
