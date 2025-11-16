using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Stakeholders.Core.Domain;

public class Monument : Entity
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int YearOfCreation { get; private set; }
    public MonumentStatus Status { get; private set; }
    public double Latitude { get; private set; }
    public double Longitude { get; private set; }

    public Monument(string name, string description, int yearOfCreation, double latitude, double longitude)
    {
        Name = name;
        Description = description;
        YearOfCreation = yearOfCreation;
        Status = MonumentStatus.Active;
        Latitude = latitude;
        Longitude = longitude;
        Validate();
    }

    private void Validate()
    {
        if (string.IsNullOrWhiteSpace(Name)) throw new ArgumentException("Invalid Name");
        if (string.IsNullOrWhiteSpace(Description)) throw new ArgumentException("Invalid Description");
        if (YearOfCreation <= 0) throw new ArgumentException("Invalid Year of Creation");
        if (Latitude < -90 || Latitude > 90) throw new ArgumentException("Invalid Latitude");
        if (Longitude < -180 || Longitude > 180) throw new ArgumentException("Invalid Longitude");
    }
}

public enum MonumentStatus
{
    Active,
    Inactive
}
