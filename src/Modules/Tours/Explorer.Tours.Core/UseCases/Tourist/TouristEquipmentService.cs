using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TouristEquipmentService : ITouristEquipmentService
{
    private readonly ITouristEquipmentRepository _touristEquipmentRepository;
    private readonly IMapper _mapper;

    public TouristEquipmentService(
        ITouristEquipmentRepository touristEquipmentRepository,
        IMapper mapper)
    {
        _touristEquipmentRepository = touristEquipmentRepository;
        _mapper = mapper;
    }

    public List<TouristEquipmentDto> GetByTourist(long touristId)
    {
        var entities = _touristEquipmentRepository.GetByTourist(touristId);
        return entities.Select(_mapper.Map<TouristEquipmentDto>).ToList();
    }

    public List<TouristEquipmentDto> UpdateForTourist(UpdateTouristEquipmentDto request)
    {
        // 1) Učitaj postojeće zapise
        var existing = _touristEquipmentRepository.GetByTourist(request.TouristId);
        var existingEquipmentIds = existing.Select(e => e.EquipmentId).ToHashSet();

        var desiredIds = request.EquipmentIds.Distinct().ToHashSet();

        // 2) Obrisi sve koje više ne treba da postoje
        foreach (var entity in existing.Where(e => !desiredIds.Contains(e.EquipmentId)))
        {
            _touristEquipmentRepository.Delete(entity.Id);
        }

        // 3) Dodaj nove zapise koje trenutno nemamo
        var toAdd = desiredIds.Except(existingEquipmentIds);
        foreach (var equipmentId in toAdd)
        {
            var entity = new TouristEquipment(request.TouristId, equipmentId);
            _touristEquipmentRepository.Create(entity);
        }

        // 4) Vrati aktuelno stanje
        var updated = _touristEquipmentRepository.GetByTourist(request.TouristId);
        return updated.Select(_mapper.Map<TouristEquipmentDto>).ToList();
    }
}
