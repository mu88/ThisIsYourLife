using System;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;

namespace WebApp
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateHostBuilder(args).Build();
            using (var serviceScope = host.Services.CreateScope())
            {
                var storage = serviceScope.ServiceProvider.GetService<Storage>();
                SeedTestData(storage);
            }

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

        private static void SeedTestData(DbContext? storage)
        {
            if (storage == null) return;

            var alice = new Person("Alice");
            var bob = new Person("Bob");
            storage.AddRange(alice, bob);

            for (var i = 0; i < 10; i++)
            {
                var random = new Random();

                var date = new DateTime(random.Next(1980, 2020), random.Next(1, 12), random.Next(1, 28));
                var caption = $"Caption {i}";
                var description = $"Description {i}";
                double latitude = random.Next(-50, 50);
                double longitude = random.Next(-90, 90);
                var createdBy = random.Next(0, 2) == 0 ? alice : bob;

                storage.Add(new LifePoint(date,
                                          caption,
                                          description,
                                          latitude,
                                          longitude,
                                          createdBy));
            }

            storage.SaveChanges();
        }
    }
}