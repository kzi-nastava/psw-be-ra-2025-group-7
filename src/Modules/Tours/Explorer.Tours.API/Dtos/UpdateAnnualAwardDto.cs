namespace Explorer.Tours.API.Dtos;

public class UpdateAnnualAwardDto
{
    public long Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Year { get; set; }
    public DateTime VotingStartDate { get; set; }
    public DateTime VotingEndDate { get; set; }
}
