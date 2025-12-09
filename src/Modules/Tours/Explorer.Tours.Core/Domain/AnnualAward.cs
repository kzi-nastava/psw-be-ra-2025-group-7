using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class AnnualAward : AggregateRoot
{
    public string Name { get; private set; }
    public string Description { get; private set; }
    public int Year { get; private set; }
    public AwardStatus Status { get; private set; }
    public DateTime VotingStartDate { get; private set; }
    public DateTime VotingEndDate { get; private set; }

    public AnnualAward(string name, string description, int year, DateTime votingStartDate, DateTime votingEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        if (year < 2000 || year > 2100)
            throw new ArgumentException("Year must be between 2000 and 2100.", nameof(year));

        if (votingStartDate >= votingEndDate)
            throw new ArgumentException("Voting start date must be before end date.");

        Name = name;
        Description = description;
        Year = year;
        Status = AwardStatus.Draft;
        VotingStartDate = votingStartDate;
        VotingEndDate = votingEndDate;
    }

    public void Update(string name, string description, int year, DateTime votingStartDate, DateTime votingEndDate)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description cannot be empty.", nameof(description));

        if (year < 2000 || year > 2100)
            throw new ArgumentException("Year must be between 2000 and 2100.", nameof(year));

        if (votingStartDate >= votingEndDate)
            throw new ArgumentException("Voting start date must be before end date.");

        Name = name;
        Description = description;
        Year = year;
        VotingStartDate = votingStartDate;
        VotingEndDate = votingEndDate;
    }

    public void ChangeStatus(AwardStatus newStatus)
    {
        Status = newStatus;
    }
}
