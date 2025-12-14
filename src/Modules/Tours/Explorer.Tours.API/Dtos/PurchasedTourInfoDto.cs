namespace Explorer.Tours.API.Dtos;

/// <summary>
/// Contains full tour information for purchased tours.
/// Tourists see partial info before purchase, and full info (without secrets) after purchase.
/// </summary>
public class PurchasedTourInfoDto
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Difficulty { get; set; }
    public decimal Price { get; set; }
    public int Status { get; set; }
    
    // Available before purchase
    public List<TourDurationDto> TourDurations { get; set; } = new();
    public List<string> Tags { get; set; } = new();
    
    // Key points revealed after purchase (without secrets)
    public List<KeyPointWithoutSecretDto> KeyPoints { get; set; } = new();
    
    // First key point serves as starting point
    public KeyPointWithoutSecretDto? StartingPoint => KeyPoints?.FirstOrDefault();
}
