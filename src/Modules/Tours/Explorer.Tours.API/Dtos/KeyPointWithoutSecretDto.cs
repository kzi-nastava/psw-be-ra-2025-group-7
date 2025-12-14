namespace Explorer.Tours.API.Dtos;

/// <summary>
/// Key point information without the secret.
/// Secrets are only revealed when tourist reaches the key point during tour execution.
/// </summary>
public class KeyPointWithoutSecretDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}
