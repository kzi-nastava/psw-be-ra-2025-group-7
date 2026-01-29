namespace Explorer.Tours.Core.Domain;

public class WeatherData
{
    public string Condition { get; private set; } // sunny, rainy, cloudy, snowy
    public double Temperature { get; private set; } // Celsius

    private WeatherData() { }

    public WeatherData(string condition, double temperature)
    {
        Condition = condition ?? throw new ArgumentNullException(nameof(condition));
        Temperature = temperature;
    }
}