namespace Explorer.Tours.API.Dtos;

/// <summary>
/// DTO za promenu statusa dodele.
/// </summary>
public class ChangeAwardStatusDto
{
    public long AwardId { get; set; }
    public string NewStatus { get; set; } // "Draft", "Active", ili "Closed"
}
