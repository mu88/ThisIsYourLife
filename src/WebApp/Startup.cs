using System.Diagnostics.CodeAnalysis;
using BusinessServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using WebApp.Services;
using WebApp.Shared;

namespace WebApp;

[ExcludeFromCodeCoverage]
public class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration;

    // R# disabled because ASP.NET Core will handle that
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    // ReSharper disable once MemberCanBePrivate.Global
    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddRazorPages();
        services.AddServerSideBlazor(options =>
        {
            options.RootComponents.RegisterAsCustomElement<LifePointDetail>("life-point-detail");
            options.RootComponents.RegisterAsCustomElement<NewLifePoint>("new-life-point");
            options.RootComponents.RegisterAsCustomElement<FilterLifePoints>("filter-life-points");
        });

        services.AddPersistence();
        services.AddBusinessServices();
        services.AddSingleton<INewLifePointDateService, NewLifePointDateService>();

        ConfigureAutoMapper(services);

        services.AddOptions<UserConfig>().Bind(Configuration);

        services.AddLocalization();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UsePathBase("/thisIsYourLife");

        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else { app.UseExceptionHandler("/Error"); }

        app.UseStaticFiles();

        app.UseRouting();

        UseRequestLocalization(app);

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });
    }

    private static void ConfigureAutoMapper(IServiceCollection services) => services.AddAutoMapper(config => config.AddProfile(typeof(AutoMapperProfile)));

    private void UseRequestLocalization(IApplicationBuilder app)
    {
        var supportedCultures = new[] { "en", "de" };
        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(supportedCultures[0])
            .AddSupportedCultures(supportedCultures)
            .AddSupportedUICultures(supportedCultures);

        app.UseRequestLocalization(localizationOptions);
    }
}