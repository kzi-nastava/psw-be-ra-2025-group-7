using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TouristEquipment : Entity
{
    public long TouristId { get; private set; }
    public long EquipmentId { get; private set; }

    // Potreban je protected konstruktor za EF
    protected TouristEquipment() { }

    public TouristEquipment(long touristId, long equipmentId)
    {
        if (touristId <= 0) throw new ArgumentException("Invalid tourist id.");
        if (equipmentId == 0) throw new ArgumentException("Invalid equipment id.");

        TouristId = touristId;
        EquipmentId = equipmentId;
    }
}
