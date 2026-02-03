using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace Explorer.Tours.Core.UseCases.ExternalServices
{
    public class WeatherService : IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["Weather:ApiKey"] ?? throw new InvalidOperationException("Weather API key not configured");
        }

        public async Task<WeatherDataDto?> GetCurrentWeather(double latitude, double longitude)
        {
            try
            {
                var url = $"https://api.openweathermap.org/data/2.5/weather?" +
                          $"lat={latitude}&lon={longitude}&" +
                          $"appid={_apiKey}&units=metric";

                var response = await _httpClient.GetAsync(url);

                if (!response.IsSuccessStatusCode)
                    return null;

                var content = await response.Content.ReadAsStringAsync();
                var weatherResponse = JsonSerializer.Deserialize<OpenWeatherResponse>(content);

                if (weatherResponse?.Weather == null || weatherResponse.Weather.Count == 0)
                    return null;

                var condition = MapWeatherCondition(weatherResponse.Weather[0].Main);
                var temperature = weatherResponse.Main?.Temp ?? 0;

                return new WeatherDataDto
                {
                    Condition = condition,
                    Temperature = temperature
                };
            }
            catch
            {
                return null;
            }
        }

        private string MapWeatherCondition(string openWeatherCondition)
        {
            return openWeatherCondition.ToLower() switch
            {
                "clear" => "sunny",
                "rain" or "drizzle" or "thunderstorm" => "rainy",
                "clouds" => "cloudy",
                "snow" => "snowy",
                _ => "cloudy"
            };
        }

        // Pomoćne klase za JSON deserializaciju
        private class OpenWeatherResponse
        {
            public List<WeatherDescription> Weather { get; set; }
            public MainData Main { get; set; }
        }

        private class WeatherDescription
        {
            public string Main { get; set; }
        }

        private class MainData
        {
            public double Temp { get; set; }
        }
    }
}
