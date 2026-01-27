using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Notifications.API.Public;
using Explorer.Stakeholders.API.Internal;
using Explorer.Stakeholders.API.Public;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System.Xml;

public class TourProblemService : ITourProblemService
{
    private readonly ITourProblemRepository _repository;
    private readonly ITourRepository _tourRepository;
    private readonly IMapper _mapper;
    private readonly Explorer.Notifications.API.Public.INotificationService _notificationService;
    //private readonly IUserProfileService _userProfileService;
    private readonly IUserInternalService _userInternalService;



    public TourProblemService(ITourProblemRepository repository, ITourRepository tourRepository,
 IMapper mapper, Explorer.Notifications.API.Public.INotificationService notificationService, IUserInternalService userInternalService)

    {
        _repository = repository;
        _tourRepository = tourRepository;
        _mapper = mapper;
        _notificationService = notificationService;
        _userInternalService = userInternalService;
    }

    public PagedResult<TourProblemDto> GetByAuthor(int authorId, int page, int pageSize)
    {
        var result = _repository.GetByAuthor(authorId, page, pageSize);
        var items = _mapper.Map<List<TourProblemDto>>(result.Results);
        foreach (var dto in items)
        {

            foreach (var comment in dto.Comments)
            {
                var username = _userInternalService.GetUsername(comment.CreatorId);
                comment.CreatorUsername = $"{username}";
            }

            var tour = _tourRepository.Get(dto.TourId);
            dto.TourName = tour.Name;
            
        }



        return new PagedResult<TourProblemDto>(items, result.TotalCount);
    }
    public PagedResult<TourProblemDto> GetTouristProblemsPages(int touristId, int page, int pageSize)
    {
        var result = _repository.GetByTourist(touristId, page, pageSize);
        var items = _mapper.Map<List<TourProblemDto>>(result.Results);
        foreach (var dto in items)
        {
            foreach (var comment in dto.Comments)
            {
                var username = _userInternalService.GetUsername(comment.CreatorId);
                comment.CreatorUsername = $"{username}";
            }

            var tour = _tourRepository.Get(dto.TourId);
            dto.TourName = tour.Name;
            dto.AuthorId = tour.AuthorId;

        }
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

        // 1) Napravi domen entitet
        var entity = new TourProblem(
            dto.TourId,
            touristId,
            Enum.Parse<ProblemCategory>(dto.Category, true),
            Enum.Parse<ProblemPriority>(dto.Priority, true),
            dto.Description,
            dto.IsSolved
        );

        dto.Comments = new();
        dto.Status = "Open";
        dto.TimeReported = DateTime.UtcNow;

        // 2) Sačuvaj u bazi
        var created = _repository.Create(entity);

        // 3) Pripremi notifikaciju za AUTORA TURE
        //    (turista je prijavio novi problem → obavesti autora)
        var tour = _tourRepository.Get(created.TourId);
        long receiverId = tour.AuthorId;   // pretpostavka: AuthorId je long

        // prvi komentar je inicijalni opis problema (dodaje se u konstruktoru)
        var firstComment = created.Comments.LastOrDefault();
        var createdAt = firstComment?.CreatedAt ?? created.TimeReported;
        var fullText = firstComment?.Message?.Trim() ?? dto.Description?.Trim() ?? string.Empty;

        var previewLength = 30;
        var preview = fullText.Length <= previewLength
            ? fullText
            : fullText.Substring(0, previewLength);

        _notificationService.CreateProblemMessageNotification(
            recipientUserId: receiverId,
            problemId: created.Id,
            messagePreview: preview,
            createdAt: createdAt
        );

        var dtoResult = _mapper.Map<TourProblemDto>(created);
        dtoResult.TourName = tour.Name;
        return dtoResult;

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

    public List<TourProblemDto> GetAll()
    {
        var tourProblems = _repository.GetAll();
        var items = _mapper.Map<List<TourProblemDto>>(tourProblems);

        foreach (var dto in items)
        {
            foreach(var comment in dto.Comments)
            {
                var username = _userInternalService.GetUsername(comment.CreatorId);
                comment.CreatorUsername = $"{username}";
            }
            var tour = _tourRepository.Get(dto.TourId);
            dto.TourName = tour.Name;
        }

        return items;
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

    public TourProblemDto ArchiveTour(int id)
    {
        var updated = _repository.ArchiveTour(id);
        return _mapper.Map<TourProblemDto>(updated);
    }

    public TourProblemDto GetById(int id)
    {
        try
        {
            var problem = _repository.Get(id);
            var dto = _mapper.Map<TourProblemDto>(problem);

            foreach (var comment in dto.Comments)
            {
                var username = _userInternalService.GetUsername(comment.CreatorId);
                comment.CreatorUsername = $"{username}";
            }

            var tour = _tourRepository.Get(dto.TourId);
            dto.TourName = tour.Name;

            return dto;
        }
        catch (KeyNotFoundException)
        {
            throw new NotFoundException("TourProblem not found.");
        }
    }

    public TourProblemDto MarkAsResolved(int id, int touristId)
    {
        var problem = _repository.Get(id);
        problem.MarkAsResolved(touristId);
        var updated = _repository.Update(problem);
        return _mapper.Map<TourProblemDto>(updated);
    }
    public TourProblemDto MarkAsUnresolved(int id, int touristId, string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Message cannot be empty.");

        // 1) Učitaj problem i uradi domenensku logiku
        var problem = _repository.Get(id);
        problem.MarkAsNotResolved(touristId, message);   // dodaje komentar + status = Unresolved

        var updated = _repository.Update(problem);

        // 2) Pripremi notifikaciju za AUTORA TURE
        //    (turista šalje poruku → notifikacija ide autoru)
        var tour = _tourRepository.Get(updated.TourId);
        long receiverId = tour.AuthorId;

        // poslednji komentar je upravo ovaj "nije rešeno" koji je turist uneo
        var lastComment = updated.Comments.Last();
        var createdAt = lastComment.CreatedAt;
        var fullText = lastComment.Message?.Trim() ?? string.Empty;

        var previewLength = 30;
        var preview = fullText.Length <= previewLength
            ? fullText
            : fullText.Substring(0, previewLength);

        _notificationService.CreateProblemMessageNotification(
            recipientUserId: receiverId,
            problemId: updated.Id,
            messagePreview: preview,
            createdAt: createdAt
        );

        return _mapper.Map<TourProblemDto>(updated);
    }



}
