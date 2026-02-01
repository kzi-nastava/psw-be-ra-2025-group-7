using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface IWeatherService
{
    Task<WeatherDataDto?> GetCurrentWeather(double latitude, double longitude);
}