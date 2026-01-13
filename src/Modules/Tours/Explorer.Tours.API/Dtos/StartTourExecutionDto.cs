namespace Explorer.Tours.API.Dtos;

public class StartTourExecutionDto
{
    public long TourId { get; set; }
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}
