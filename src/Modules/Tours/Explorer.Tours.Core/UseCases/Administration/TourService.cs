using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Explorer.BuildingBlocks.Core.Exceptions;

namespace Explorer.Tours.Core.UseCases.Administration;

public class TourService : ITourService
{
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;

    public TourService(ITourRepository repository, IMapper mapper)
    {
        _tourRepository = repository;
        _mapper = mapper;
    }

    public PagedResult<TourDto> GetPagedByAuthor(int page, int pageSize, long authorId)
    {
        var result = _tourRepository.GetPagedByAuthor(page, pageSize,authorId);

        var items = result.Results.Select(_mapper.Map<TourDto>).ToList();
        return new PagedResult<TourDto>(items, result.TotalCount);
    }

    public TourDto Create(TourDto entity)
    {
        var result = _tourRepository.Create(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public TourDto Update(TourDto entity)
    {
        var result = _tourRepository.Update(_mapper.Map<Tour>(entity));
        return _mapper.Map<TourDto>(result);
    }

    public void Delete(long id, long authorId)
    {
        var tour = _tourRepository.Get(id);

        if (tour.AuthorId != authorId)
            throw new ForbiddenException("You can only delete your own tours.");

        if (tour.Status != TourStatus.Draft)
            throw new InvalidOperationException("Only Draft tours can be deleted.");

        _tourRepository.Delete(id);
    }
}