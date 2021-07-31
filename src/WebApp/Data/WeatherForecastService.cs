using System;
using System.Linq;
using System.Threading.Tasks;
using Entities;
using Persistence;

namespace WebApp.Data
{
    public class WeatherForecastService
    {
        private readonly Storage _storage;

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        public WeatherForecastService(Storage storage)
        {
            _storage = storage;
        }

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            _storage.Add(new Person($"Robot{new Random().Next(0, 50)}"));
            _storage.SaveChanges();
            
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            }).ToArray());
        }
    }
}
