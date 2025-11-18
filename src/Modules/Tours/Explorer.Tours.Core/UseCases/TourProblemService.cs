using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

public class TourProblemService : ITourProblemService
{
    private readonly ITourProblemRepository _repository;
    private readonly IMapper _mapper;

    public TourProblemService(ITourProblemRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public PagedResult<TourProblemDto> GetTouristProblemsPages(int touristId, int page, int pageSize)
    {
        var result = _repository.GetByTourist(touristId, page, pageSize);
        var items = _mapper.Map<List<TourProblemDto>>(result.Results);
        return new PagedResult<TourProblemDto>(items, result.TotalCount);
    }

    public TourProblemDto Create(TourProblemDto dto, int touristId)
    {
        if (dto.TourId <= 0)
            throw new ArgumentException("TourId is required.");

        if (string.IsNullOrWhiteSpace(dto.Category))
            throw new ArgumentException("Category is required.");

        if (string.IsNullOrWhiteSpace(dto.Priority))
            throw new ArgumentException("Priority is required.");

        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Description is required.");

        dto.TouristId = touristId;
        if (dto.TimeReported == default) dto.TimeReported = DateTime.UtcNow;

        var entity = _mapper.Map<TourProblem>(dto);
        var created = _repository.Create(entity);
        return _mapper.Map<TourProblemDto>(created);
    }

    public TourProblemDto Update(TourProblemDto dto, int touristId)
    {
        TourProblem existing;

        try
        {
            existing = _repository.Get(dto.Id);
        }
        catch (KeyNotFoundException)
        {
            throw new NotFoundException("TourProblem not found.");
        }

        if (existing.TouristId != touristId)
            throw new UnauthorizedAccessException("Cannot update another user's problem report.");

        var entity = _mapper.Map<TourProblem>(dto);
        var updated = _repository.Update(entity);
        return _mapper.Map<TourProblemDto>(updated);
    }


    public void Delete(int id, int touristId)
    {
        // get throws NotFoundException if not exists
        var problem = _repository.Get(id);

        // owner check
        if (problem.TouristId != touristId)
            throw new UnauthorizedAccessException("Forbidden: not the owner");

        _repository.Delete(id);
    }
}
