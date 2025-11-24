using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITouristEquipmentService
{

    List<EquipmentDto> GetAllEquipment();


    List<TouristEquipmentDto> GetByTourist(long touristId);


    List<TouristEquipmentDto> UpdateForTourist(UpdateTouristEquipmentDto request);

    void AddEquipmentToTourist(long touristId, long equipmentId);

    void RemoveEquipmentFromTourist(long touristEquipmentId);

}
