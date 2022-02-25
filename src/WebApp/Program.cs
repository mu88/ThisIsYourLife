using System;
using System.IO;
using Entities;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Persistence;

namespace WebApp;

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
            .ConfigureAppConfiguration(builder => builder.AddJsonFile(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "user.json"), true))
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });

    private static void SeedTestData(DbContext? storage)
    {
        if (storage == null) return;

        var alice = new Person("Alice");
        var bob = new Person("Bob");
        storage.AddRange(alice, bob);

        for (var i = 0; i < 500; i++)
        {
            var random = new Random();

            var date = new DateOnly(random.Next(1980, 2020), random.Next(1, 12), random.Next(1, 28));
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

        storage.Add(new LifePoint(new DateOnly(1953, 4, 12),
                                  "Home of Football",
                                  "Nur die SGD",
                                  51.0405849,
                                  13.7478431,
                                  alice));

        storage.SaveChanges();
    }
}