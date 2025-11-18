using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Tourist;

public class TourJournalService : ITourJournalService
{
    private readonly ITourJournalRepository _tourJournalRepository;
    private readonly IMapper _mapper;

    public TourJournalService(ITourJournalRepository repository, IMapper mapper)
    {
        _tourJournalRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<TourJournalDto> GetPagedByTourist(long touristId, int page, int pageSize)
    {
        var result = _tourJournalRepository.GetPagedByTourist(touristId, page, pageSize);
        var items = result.Results.Select(_mapper.Map<TourJournalDto>).ToList();
        return new PagedResult<TourJournalDto>(items, result.TotalCount);
    }

    public TourJournalDto Create(TourJournalDto tourJournalDto)
    {
        var tourJournal = new TourJournal(
            tourJournalDto.TouristId,
            tourJournalDto.Name,
            tourJournalDto.Country,
            tourJournalDto.City
        );
        
        var result = _tourJournalRepository.Create(tourJournal);
        return _mapper.Map<TourJournalDto>(result);
    }

    public TourJournalDto Update(TourJournalDto tourJournalDto)
    {
        var tourJournal = _tourJournalRepository.Get(tourJournalDto.Id);
        
        // Provera vlasništva
        if (tourJournal.TouristId != tourJournalDto.TouristId)
        {
            throw new UnauthorizedAccessException("You are not authorized to update this tour journal.");
        }
        
        tourJournal.Update(
            tourJournalDto.Name,
            tourJournalDto.Country,
            tourJournalDto.City
        );
        
        var result = _tourJournalRepository.Update(tourJournal);
        return _mapper.Map<TourJournalDto>(result);
    }

    public void Delete(long id, long touristId)
    {
        var tourJournal = _tourJournalRepository.Get(id);
        
        // Provera vlasništva
        if (tourJournal.TouristId != touristId)
        {
            throw new UnauthorizedAccessException("You are not authorized to delete this tour journal.");
        }
        
        _tourJournalRepository.Delete(id);
    }
}
