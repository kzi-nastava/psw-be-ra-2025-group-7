namespace Explorer.Stakeholders.API.Dtos;

public class MonumentDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int YearOfCreation { get; set; }
    public int Status { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
