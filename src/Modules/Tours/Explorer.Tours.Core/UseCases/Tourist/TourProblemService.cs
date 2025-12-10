using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Notifications.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

public class TourProblemService : ITourProblemService
{
    private readonly ITourProblemRepository _repository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly INotificationService _notificationService;

    public TourProblemService(ITourProblemRepository repository, ITourRepository tourRepository,
 IMapper mapper, INotificationService notificationService)

    {
        _repository = repository;
        _tourRepository = tourRepository;
        _mapper = mapper;
        _notificationService = notificationService;

    }

    public PagedResult<TourProblemDto> GetByAuthor(int authorId, int page, int pageSize)
    {
        var result = _repository.GetByAuthor(authorId, page, pageSize);
        var items = _mapper.Map<List<TourProblemDto>>(result.Results);
        return new PagedResult<TourProblemDto>(items, result.TotalCount);
    }
    public PagedResult<TourProblemDto> GetTouristProblemsPages(int touristId, int page, int pageSize)
    {
        var result = _repository.GetByTourist(touristId, page, pageSize);
        var items = _mapper.Map<List<TourProblemDto>>(result.Results);
        return new PagedResult<TourProblemDto>(items, result.TotalCount);
    }

    public TourProblemDto Create(TourProblemDto dto, int touristId)
    {
      
        if (string.IsNullOrWhiteSpace(dto.Category) ||
            string.IsNullOrWhiteSpace(dto.Priority) ||
            string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Missing fields.");

        dto.TouristId = touristId;
        if (dto.TimeReported == default)
            dto.TimeReported = DateTime.UtcNow;


        var entity = new TourProblem(
        dto.TourId,
        touristId,
        Enum.Parse<ProblemCategory>(dto.Category, true),
        Enum.Parse<ProblemPriority>(dto.Priority, true),
        dto.Description,dto.IsSolved
    );
        dto.Comments = new();
        dto.Status = "Open";
        dto.TimeReported = DateTime.UtcNow;
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

        dto.TimeReported = DateTime.UtcNow;

        var entity = _mapper.Map<TourProblem>(dto);
        try
        {
            var updated = _repository.Update(entity);
            return _mapper.Map<TourProblemDto>(updated);
        }
        catch (KeyNotFoundException)
        {
            throw new NotFoundException("TourProblem not found.");
        }
    }

    public TourProblemDto AddAuthorReply(int tourProblemId, int authorId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.");

        var problem = _repository.Get(tourProblemId);

        // 1) Dodaj poruku u problem (ovo i dalje radi domen, ne diramo)
        problem.AddAuthorReply(authorId, message);

        var updated = _repository.Update(problem);

        // 2) Odredi ko je primalac (druga strana)
        // sender = authorId
        long receiverId;

        if (authorId == updated.TouristId)
        {
            // poruku šalje TURISTA → notifikacija ide AUTORU TURE
            var tour = _tourRepository.Get(updated.TourId);
            receiverId = tour.AuthorId;
        }
        else
        {
            // poruku šalje AUTOR / ADMIN / bilo ko “sa druge strane” → notifikacija ide TURISTI
            receiverId = updated.TouristId;
        }

        // 3) Pripremi preview poruke i vreme
        var lastComment = updated.Comments.Last();      // upravo dodata poruka
        var createdAt = lastComment.CreatedAt;
        var fullText = lastComment.Message?.Trim() ?? string.Empty;

        var previewLength = 30;
        var preview = fullText.Length <= previewLength
            ? fullText
            : fullText.Substring(0, previewLength);

        // 4) Kreiraj notifikaciju preko servisa
        _notificationService.CreateProblemMessageNotification(
            recipientUserId: receiverId,
            problemId: updated.Id,
            messagePreview: preview,
            createdAt: createdAt
        );

        return _mapper.Map<TourProblemDto>(updated);
    }


    public void Delete(int id, int touristId)
    {
        var problem = _repository.Get(id);
        if (problem.TouristId != touristId)
            throw new UnauthorizedAccessException();

        _repository.Delete(id);
    }

    public List<TourProblemDto> GetAll() {
    var tourProblems = _repository.GetAll();
    return _mapper.Map<List<TourProblemDto>>(tourProblems);
    }

    public TourProblemDto SetResolveDue(int id, string date)
    {
        var parsed = DateTime.Parse(date);

        var updated = _repository.UpdateResolveDue(id, parsed);

        return _mapper.Map<TourProblemDto>(updated);
    }

    public TourProblemDto SetPenalty(int id)
    {
        var updated = _repository.SetPenalty(id);
        return _mapper.Map<TourProblemDto>(updated);
    }
    public TourProblemDto MarkAsResolved(int id, int touristId)
    {
        var problem = _repository.Get(id);
        problem.MarkAsResolved(touristId);
        var updated = _repository.Update(problem);
        return _mapper.Map<TourProblemDto>(updated);
    }  
    public TourProblemDto MarkAsUnresolved(int id, int touristId,string message)
    {
        if(string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.");
        var problem = _repository.Get(id);
        problem.MarkAsNotResolved(touristId,message);
        var updated = _repository.Update(problem);
        return _mapper.Map<TourProblemDto>(updated);
    }

     
}
