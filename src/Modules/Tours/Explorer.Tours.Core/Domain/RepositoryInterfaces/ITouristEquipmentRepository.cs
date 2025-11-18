using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface ITouristEquipmentRepository
{
    /// <summary>Vraća sve zapise za datog turistu.</summary>
    List<TouristEquipment> GetByTourist(long touristId);

    /// <summary>Upisuje novi zapis (turista–oprema).</summary>
    TouristEquipment Create(TouristEquipment entity);

    /// <summary>Briše jedan zapis po njegovom Id-ju.</summary>
    void Delete(long id);

    IEnumerable<Equipment> GetAll();


}
