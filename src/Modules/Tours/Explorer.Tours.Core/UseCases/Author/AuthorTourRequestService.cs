using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Author;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Author
{
    public class AuthorTourRequestService : IAuthorTourRequestService
    {
        private readonly ITourRequestRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserInternalService _userInternalService;
        private readonly ITourRepository _tourRepo;

        public AuthorTourRequestService(
            ITourRequestRepository repository,
            IMapper mapper,
            IUserInternalService userInternalService,
            ITourRepository tourRepo)
        {
            _repository = repository;
            _mapper = mapper;
            _userInternalService = userInternalService;
            _tourRepo = tourRepo;
        }

        public PagedResult<AuthorTourRequestListItemDto> GetOpen(
            int page,
            int pageSize,
            long authorId,
            decimal? minBudget,
            decimal? maxBudget, int? difficulty)
        {
            var result = _repository.GetOpenRequestsFiltered(page, pageSize, minBudget, maxBudget, difficulty);

            var dtos = result.Results.Select(tr =>
            {
                var dto = _mapper.Map<AuthorTourRequestListItemDto>(tr);

                dto.ResponseCount = _repository.GetResponseCount(tr.Id);
                dto.DaysUntilExpiration = tr.DaysUntilExpiration();
                dto.AlreadyResponded = _repository.GetResponseByAuthorAndRequest(authorId, tr.Id) != null;

                return dto;
            }).ToList();

            return new PagedResult<AuthorTourRequestListItemDto>(dtos, result.TotalCount);
        }

        public AuthorTourRequestDetailsDto GetDetails(long requestId, long authorId)
        {
            var tr = _repository.Get(requestId);

            if (tr.Status == TourRequestStatus.Closed || tr.Status == TourRequestStatus.Fulfilled)
                throw new InvalidOperationException("This request is no longer accepting responses.");

            var dto = _mapper.Map<AuthorTourRequestDetailsDto>(tr);

            dto.ResponseCount = _repository.GetResponseCount(tr.Id);
            dto.DaysUntilExpiration = tr.DaysUntilExpiration();

            var tourist = _userInternalService.GetUserBasicInfo(tr.TouristId);
            dto.TouristName = tourist.DisplayName;
            dto.TouristProfilePicture = tourist.ProfilePicture;

            dto.ContactDisclaimer = "You only get contact information if the tourist accepts your offer.";
            return dto;
        }

        public TourRequestResponseDto CreateResponse(CreateTourRequestResponseDto dto, long authorId)
        {
            var request = _repository.Get(dto.TourRequestId);

            if (request.Status == TourRequestStatus.Closed || request.Status == TourRequestStatus.Fulfilled)
                throw new InvalidOperationException("This request is no longer accepting responses.");

            if (request.ExpiresAt <= DateTime.UtcNow)
                throw new InvalidOperationException("This request has expired.");

            var existing = _repository.GetResponseByAuthorAndRequest(authorId, dto.TourRequestId);
            if (existing != null)
                throw new InvalidOperationException("You have already responded to this request.");

            if (!dto.TourId.HasValue)
            {
                throw new ArgumentException(
                    dto.ResponseType == (int)ResponseType.CustomProposal
                        ? "You must create a draft tour before sending a custom proposal."
                        : "Tour ID is required.");
            }

            var tour = _tourRepo.Get(dto.TourId.Value);

            if (tour.AuthorId != authorId)
                throw new UnauthorizedAccessException("You can only offer your own tours.");

            TourRequestResponse response;
            var responseType = (ResponseType)dto.ResponseType;

            if (responseType == ResponseType.ExistingTour)
            {
                if (tour.Status != TourStatus.Published)
                    throw new InvalidOperationException("Only published tours can be offered as existing tours.");

                response = TourRequestResponse.CreateForExistingTour(
                    dto.TourRequestId,
                    authorId,
                    dto.TourId.Value,
                    dto.ProposedPrice,
                    dto.Message
                );
            }
            else if (responseType == ResponseType.CustomProposal)
            {
                if (tour.Status != TourStatus.Draft)
                    throw new InvalidOperationException("Custom proposals must use draft tours.");

                if (string.IsNullOrWhiteSpace(dto.ProposalDescription))
                    throw new ArgumentException("Proposal description is required for custom proposals.");

                response = TourRequestResponse.CreateCustomProposal(
                    dto.TourRequestId,
                    authorId,
                    dto.TourId.Value,      
                    dto.ProposalDescription,
                    dto.ProposedPrice,
                    dto.Message
                );
            }
            else
            {
                throw new ArgumentException("Invalid response type.");
            }

            var created = _repository.CreateResponse(response);

            if (request.Status == TourRequestStatus.Open)
            {
                request.MarkInProgress();
                _repository.Update(request);
            }

            return _mapper.Map<TourRequestResponseDto>(created);
        }

        public List<AuthorResponseItemDto> GetMyResponses(long authorId)
        {
            var responses = _repository.GetResponsesByAuthor(authorId);
            var result = new List<AuthorResponseItemDto>();

            foreach (var response in responses)
            {
                var request = _repository.Get(response.TourRequestId);

                var dto = new AuthorResponseItemDto
                {
                    ResponseId = response.Id,
                    TourRequestId = response.TourRequestId,
                    TourRequestTitle = request.Title,
                    ResponseType = (int)response.ResponseType,
                    ProposedPrice = response.ProposedPrice,
                    SentAt = response.CreatedAt,
                    Status = (int)response.Status,
                    Message = response.Message,
                    ProposalDescription = response.ResponseType == ResponseType.CustomProposal
                        ? response.ProposalDescription
                        : null,
                    TourId = response.TourId
                };

                if (response.TourId.HasValue)
                {
                    try
                    {
                        var tour = _tourRepo.Get(response.TourId.Value);
                        if (tour != null)
                        {
                            dto.TourName = tour.Name;
                            dto.TourStatus = (int)tour.Status;
                        }
                        else
                        {
                            dto.TourName = "[Tour not found]";
                        }
                    }
                    catch
                    {
                        dto.TourName = "[Tour not found]";
                    }
                }

                try
                {
                    var basic = _userInternalService.GetUserBasicInfo(request.TouristId);

                    if (basic != null)
                    {
                        dto.TouristName = basic.DisplayName ?? $"Tourist #{request.TouristId}";
                        dto.TouristProfilePicture = basic.ProfilePicture;
                    }
                    else
                    {
                        dto.TouristName = $"Tourist #{request.TouristId}";
                        dto.TouristProfilePicture = null;
                    }

                    if (response.Status == ResponseStatus.Accepted)
                    {
                        try
                        {
                            var contact = _userInternalService.GetUserContactInfo(request.TouristId);
                            if (contact != null)
                            {
                                dto.TouristEmail = contact.Email;
                                dto.TouristBiography = contact.Biography;
                                dto.TouristMotto = contact.Motto;
                            }
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Debug.WriteLine($"Failed to load contact info: {ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Failed to load tourist info for user {request.TouristId}: {ex.Message}");
                    dto.TouristName = $"Tourist #{request.TouristId}";
                    dto.TouristProfilePicture = null;
                }

                result.Add(dto);
            }

            return result;
        }

        public TourRequestResponseDto UpdateMyResponse(long responseId, long authorId, UpdateMyResponseDto dto)
        {
            var response = _repository.GetResponse(responseId);

            if (response.AuthorId != authorId)
                throw new UnauthorizedAccessException("You can edit only your own responses.");

            if (response.Status != ResponseStatus.Pending)
                throw new InvalidOperationException("You cannot edit accepted or rejected responses.");

            if (dto.ProposedPrice <= 0)
                throw new ArgumentException("Proposed price must be greater than zero.");

            response.Update(dto.ProposedPrice, dto.Message);

            if (response.ResponseType == ResponseType.CustomProposal)
            {
                if (!string.IsNullOrWhiteSpace(dto.ProposalDescription))
                {
                    response.UpdateProposalDescription(dto.ProposalDescription);
                }

                if (dto.TourId.HasValue)
                {
                    var tour = _tourRepo.Get(dto.TourId.Value);

                    if (tour.AuthorId != authorId)
                        throw new UnauthorizedAccessException("You can only offer your own tours.");

                    if (tour.Status != TourStatus.Draft)
                        throw new InvalidOperationException("Custom proposals must use draft tours.");

                    response.UpdateTour(dto.TourId.Value);
                }
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dto.ProposalDescription))
                    throw new ArgumentException("Proposal description is not allowed for existing tour responses.");

                if (dto.TourId.HasValue)
                {
                    var tour = _tourRepo.Get(dto.TourId.Value);

                    if (tour.AuthorId != authorId)
                        throw new UnauthorizedAccessException("You can only offer your own tours.");

                    if (tour.Status != TourStatus.Published)
                        throw new InvalidOperationException("Only published tours can be offered as existing tours.");

                    response.UpdateTour(dto.TourId.Value);
                }
            }

            var updated = _repository.UpdateResponse(response);
            return _mapper.Map<TourRequestResponseDto>(updated);
        }

        public void DeleteMyResponse(long responseId, long authorId)
        {
            var response = _repository.GetResponse(responseId);

            if (response.AuthorId != authorId)
                throw new UnauthorizedAccessException("You can delete only your own responses.");

            if (response.Status != ResponseStatus.Pending)
                throw new InvalidOperationException("You cannot delete accepted or rejected responses.");

            _repository.DeleteResponse(responseId);
        }

        public void MarkResponseAsReady(long responseId, long authorId)
        {
            var response = _repository.GetResponse(responseId);

            if (response.AuthorId != authorId)
                throw new UnauthorizedAccessException("You can only mark your own responses as ready.");

            if (response.ResponseType != ResponseType.CustomProposal)
                throw new InvalidOperationException("Only custom proposals can be marked as ready.");

            if (!response.TourId.HasValue)
                throw new InvalidOperationException("Tour must be created before marking as ready.");

            var tour = _tourRepo.Get(response.TourId.Value);
            if (tour.Status != TourStatus.Published)
                throw new InvalidOperationException("Tour must be published before marking response as ready.");

            _repository.MarkResponseAsReady(responseId);

            // TODO: Pošalji notifikaciju turistu
            // _notificationService.NotifyTourist(tourRequest.TouristId, "Your custom tour is ready!");
        }
    }
}