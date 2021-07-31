using System;
using System.Linq;
using System.Threading.Tasks;
using BusinessServices;
using Entities;

namespace WebApp.Data
{
    public class WeatherForecastService
    {
        private static readonly string[] Summaries =
        {
            "Freezing",
            "Bracing",
            "Chilly",
            "Cool",
            "Mild",
            "Warm",
            "Balmy",
            "Hot",
            "Sweltering",
            "Scorching"
        };

        private readonly IStorage _storage;

        public WeatherForecastService(IStorage storage) => _storage = storage;

        public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            await _storage.AddItemAsync(new Person($"Robot{new Random().Next(0, 50)}"));
            await _storage.SaveAsync();
            var count = _storage.Persons.Count();

            var rng = new Random();
            return Enumerable.Range(1, 5)
                .Select(index => new WeatherForecast
                {
                    Date = startDate.AddDays(index), TemperatureC = rng.Next(-20, 55), Summary = Summaries[rng.Next(Summaries.Length)]
                })
                .ToArray();
        }
    }
}