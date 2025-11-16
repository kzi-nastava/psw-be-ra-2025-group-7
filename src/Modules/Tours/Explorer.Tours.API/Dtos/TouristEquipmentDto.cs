namespace Explorer.Tours.API.Dtos;

public class TouristEquipmentDto
{
    public long Id { get; set; }
    public long TouristId { get; set; }
    public long EquipmentId { get; set; }
}


public class UpdateTouristEquipmentDto
{
    public long TouristId { get; set; }
    public List<long> EquipmentIds { get; set; } = new();
}
