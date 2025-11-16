using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Tourist;

public interface ITouristEquipmentService
{
 
    List<TouristEquipmentDto> GetByTourist(long touristId);


    List<TouristEquipmentDto> UpdateForTourist(UpdateTouristEquipmentDto request);
}
