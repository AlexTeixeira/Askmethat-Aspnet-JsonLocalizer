using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Localization;

namespace Askmethat.Aspnet.JsonLocalizer.Sample.I18nTest.Data
{
    public class WeatherForecastService
    {
        private readonly IStringLocalizer _localizer;
        public WeatherForecastService(IStringLocalizer localizer)
        {
            _localizer = localizer;
            _summaries.Add(_localizer.GetString("Freezing"));
            _summaries.Add(_localizer.GetString("Bracing"));
            _summaries.Add(_localizer.GetString("Chilly"));
            _summaries.Add(_localizer.GetString("Cool"));
            _summaries.Add(_localizer.GetString("Mild"));
            _summaries.Add(_localizer.GetString("Warm"));
        }
        
        private readonly List<string> _summaries = new List<string>();

        public Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
        {
            var rng = new Random();
            return Task.FromResult(Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = startDate.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = _summaries[rng.Next(_summaries.Count)]
            }).ToArray());
        }
    }
}