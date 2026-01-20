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
                throw new InvalidOperationException("This request is no longer accepting responses.");

            var existing = _repository.GetResponseByAuthorAndRequest(authorId, dto.TourRequestId);
            if (existing != null)
                throw new InvalidOperationException("You have already responded to this request.");

            TourRequestResponse response;
            var responseType = (ResponseType)dto.ResponseType;

            if (responseType == ResponseType.ExistingTour)
            {
                if (!dto.TourId.HasValue)
                    throw new ArgumentException("TourId is required for existing tour response.");

                var tour = _tourRepo.Get(dto.TourId.Value);

                if (tour.AuthorId != authorId)
                    throw new UnauthorizedAccessException("You can offer only your own tours.");

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
                if (string.IsNullOrWhiteSpace(dto.ProposalDescription))
                    throw new ArgumentException("Proposal description is required.");

                response = TourRequestResponse.CreateCustomProposal(
                    dto.TourRequestId,
                    authorId,
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
                    TourId = response.ResponseType == ResponseType.ExistingTour
                        ? response.TourId
                        : null
                };

                if (response.ResponseType == ResponseType.ExistingTour && response.TourId.HasValue)
                {
                    var tour = _tourRepo.Get(response.TourId.Value);
                    dto.TourName = tour.Name; 
                }

                var basic = _userInternalService.GetUserBasicInfo(request.TouristId);
                dto.TouristName = basic.DisplayName;
                dto.TouristProfilePicture = basic.ProfilePicture;

                if (response.Status == ResponseStatus.Accepted)
                {
                    var contact = _userInternalService.GetUserContactInfo(request.TouristId);
                    dto.TouristEmail = contact.Email;
                    dto.TouristBiography = contact.Biography;
                    dto.TouristMotto = contact.Motto;
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
                if (dto.ProposalDescription != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.ProposalDescription))
                        throw new ArgumentException("Proposal description cannot be empty.");

                    response.UpdateProposalDescription(dto.ProposalDescription);
                }

                if (dto.TourId.HasValue)
                    throw new ArgumentException("TourId is not allowed for custom proposal responses.");
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(dto.ProposalDescription))
                    throw new ArgumentException("Proposal description is not allowed for existing tour responses.");

                if (dto.TourId.HasValue)
                {
                    var tour = _tourRepo.Get(dto.TourId.Value);

                    if (tour.AuthorId != authorId)
                        throw new UnauthorizedAccessException("You can offer only your own tours.");

                    response.UpdateTour(dto.TourId.Value);
                }
                else
                {
                    
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
    }
}
