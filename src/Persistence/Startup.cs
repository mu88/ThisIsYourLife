using BusinessServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Persistence
{
    public static class Startup
    {
        public static void AddPersistence(this IServiceCollection services)
        {
            services.AddDbContext<Storage>(options => options.UseInMemoryDatabase("MyDatabase"));
            services.AddScoped<IStorage>(provider => provider.GetService<Storage>()!); // cannot be null since it is registered before
            services.AddSingleton<IFileSystem, FileSystem>();
        }
    }
}