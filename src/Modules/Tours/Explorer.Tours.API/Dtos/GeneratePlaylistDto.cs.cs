namespace Explorer.Tours.API.Dtos;

public class GeneratePlaylistDto
{
    public List<string> Genres { get; set; } = new();
    public bool IncludeWeather { get; set; } = false;
}