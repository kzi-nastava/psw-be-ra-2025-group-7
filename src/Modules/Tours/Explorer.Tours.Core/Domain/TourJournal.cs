using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourJournal : Entity
{
    public long TouristId { get; init; }
    public string Name { get; private set; }
    public DateTime CreatedAt { get; init; }
    public TourJournalStatus Status { get; private set; }
    public string? City { get; private set; }
    public string Country { get; private set; }

    public TourJournal(long touristId, string name, string country, string? city = null)
    {
        if (touristId == 0) throw new ArgumentException("Invalid TouristId.");
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Invalid Country.");
        
        TouristId = touristId;
        Name = name;
        Country = country;
        City = city;
        CreatedAt = DateTime.UtcNow;
        Status = TourJournalStatus.Draft;
    }

    public void Update(string name, string country, string? city)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Invalid Name.");
        if (string.IsNullOrWhiteSpace(country)) throw new ArgumentException("Invalid Country.");
        
        Name = name;
        Country = country;
        City = city;
    }

    public void UpdateStatus(TourJournalStatus status)
    {
        Status = status;
    }
}

public enum TourJournalStatus
{
    Draft,
    Published,
    Archived
}
