using BusinessServices;
using BusinessServices.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;
using WebApp.Shared;

namespace WebApp;

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
        services.AddServerSideBlazor(options => { options.RootComponents.RegisterAsCustomElement<LifePointDetail>("life-point-detail"); });
        services.AddServerSideBlazor(options => { options.RootComponents.RegisterAsCustomElement<NewLifePoint>("new-life-point"); });

        services.AddPersistence();
        services.AddBusinessServices();

        ConfigureAutoMapper(services);

        services.AddOptions<UserConfig>().Bind(Configuration);
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
            app.UseDeveloperExceptionPage();
        else
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapBlazorHub();
            endpoints.MapFallbackToPage("/_Host");
            endpoints.MapControllers();
        });
    }

    private static void ConfigureAutoMapper(IServiceCollection services) => services.AddAutoMapper(config => config.AddProfile(typeof(AutoMapperProfile)));
}