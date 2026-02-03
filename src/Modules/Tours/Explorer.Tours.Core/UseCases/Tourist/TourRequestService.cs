using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Stakeholders.API.Internal;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Core.UseCases.Tourist
{
    public class TourRequestService : ITourRequestService
    {
        private readonly ITourRequestRepository _repository;
        private readonly IMapper _mapper;
        private readonly IUserInternalService _userInternalService;
        private readonly ITourRepository _tourRepository;

        public TourRequestService(
            ITourRequestRepository repository,
            IMapper mapper,
            IUserInternalService userInternalService,
            ITourRepository tourRepository)
        {
            _repository = repository;
            _mapper = mapper;
            _userInternalService = userInternalService;
            _tourRepository = tourRepository;
        }

        public TourRequestDto Create(CreateTourRequestDto dto, long touristId)
        {
            var todayCount = _repository.GetRequestCountForToday(touristId);
            if (todayCount >= 5)
                throw new InvalidOperationException("Daily request limit reached. Please try again tomorrow.");

            var tourRequest = new TourRequest(
                touristId,
                dto.Title,
                dto.Description,
                dto.Budget);

            if (dto.Latitude.HasValue && dto.Longitude.HasValue && dto.Radius.HasValue)
            {
                tourRequest.SetLocation(dto.Latitude.Value, dto.Longitude.Value, dto.Radius.Value);
            }

            if (dto.PreferredDifficulty.HasValue)
            {
                tourRequest.SetPreferredDifficulty((TourDifficulty)dto.PreferredDifficulty.Value);
            }

            if (dto.NumberOfParticipants > 1)
            {
                tourRequest.SetNumberOfParticipants(dto.NumberOfParticipants);
            }

            if (dto.PreferredDate.HasValue)
            {
                tourRequest.SetPreferredDate(dto.PreferredDate.Value);
            }

            var created = _repository.Create(tourRequest);
            var result = _mapper.Map<TourRequestDto>(created);
            result.ResponseCount = 0;
            result.DaysUntilExpiration = created.DaysUntilExpiration();

            return result;
        }

        public TourRequestDto Update(UpdateTourRequestDto dto, long touristId)
        {
            var existing = _repository.Get(dto.Id);

            if (existing.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to update this tour request.");

            existing.Update(dto.Title, dto.Description, dto.Budget);

            if (dto.Latitude.HasValue && dto.Longitude.HasValue && dto.Radius.HasValue)
            {
                existing.SetLocation(dto.Latitude.Value, dto.Longitude.Value, dto.Radius.Value);
            }
            else
            {
                existing.ClearLocation();
            }

            if (dto.PreferredDifficulty.HasValue)
            {
                existing.SetPreferredDifficulty((TourDifficulty)dto.PreferredDifficulty.Value);
            }
            else
            {
                existing.ClearPreferredDifficulty();
            }

            existing.SetNumberOfParticipants(dto.NumberOfParticipants);

            if (dto.PreferredDate.HasValue)
            {
                existing.SetPreferredDate(dto.PreferredDate.Value);
            }
            else
            {
                existing.ClearPreferredDate();
            }

            var updated = _repository.Update(existing);
            var result = _mapper.Map<TourRequestDto>(updated);
            result.ResponseCount = _repository.GetResponseCount(updated.Id);
            result.DaysUntilExpiration = updated.DaysUntilExpiration();

            return result;
        }

        public void Delete(long id, long touristId)
        {
            var existing = _repository.Get(id);

            if (existing.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to delete this tour request.");

            if (existing.Status != TourRequestStatus.Open && existing.Status != TourRequestStatus.Closed)
                throw new InvalidOperationException("Only Open or Closed requests can be deleted.");

            _repository.Delete(id);
        }

        public PagedResult<TourRequestDto> GetByTourist(int page, int pageSize, long touristId)
        {
            var result = _repository.GetPagedByTourist(page, pageSize, touristId);

            var dtos = result.Results.Select(tr =>
            {
                var dto = _mapper.Map<TourRequestDto>(tr);
                dto.ResponseCount = _repository.GetResponseCount(tr.Id);
                dto.DaysUntilExpiration = tr.DaysUntilExpiration();
                return dto;
            }).ToList();

            return new PagedResult<TourRequestDto>(dtos, result.TotalCount);
        }

        public TourRequestDto GetById(long id, long touristId)
        {
            var tourRequest = _repository.Get(id);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to view this tour request.");

            var dto = _mapper.Map<TourRequestDto>(tourRequest);
            dto.ResponseCount = _repository.GetResponseCount(tourRequest.Id);
            dto.DaysUntilExpiration = tourRequest.DaysUntilExpiration();

            return dto;
        }

        public TourRequestDto Close(long id, long touristId)
        {
            var tourRequest = _repository.Get(id);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to close this tour request.");

            tourRequest.Close();
            var updated = _repository.Update(tourRequest);

            var dto = _mapper.Map<TourRequestDto>(updated);
            dto.ResponseCount = _repository.GetResponseCount(updated.Id);
            dto.DaysUntilExpiration = updated.DaysUntilExpiration();

            return dto;
        }

        public List<TourRequestResponseDto> GetResponses(long tourRequestId, long touristId)
        {
            var tourRequest = _repository.Get(tourRequestId);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to view responses for this tour request.");

            var responses = _repository.GetResponsesByRequest(tourRequestId);
            var dtos = _mapper.Map<List<TourRequestResponseDto>>(responses);

            foreach (var dto in dtos)
            {
                var response = responses.First(r => r.Id == dto.Id);

                try
                {
                    var authorInfo = _userInternalService.GetUserBasicInfo(response.AuthorId);

                    if (authorInfo != null)
                    {
                        dto.AuthorName = authorInfo.DisplayName ?? $"Author #{response.AuthorId}";
                        dto.AuthorAvatar = authorInfo.ProfilePicture;
                    }
                    else
                    {
                        dto.AuthorName = $"Author #{response.AuthorId}";
                        dto.AuthorAvatar = null;
                    }
                }
                catch (Exception ex)
                {
                    dto.AuthorName = $"Author #{response.AuthorId}";
                    dto.AuthorAvatar = null;
                }

                if (response.TourId.HasValue)
                {
                    try
                    {
                        var tour = _tourRepository.GetForPreview(response.TourId.Value);

                        if (tour != null)
                        {
                            dto.Tour = _mapper.Map<TourPreviewDto>(tour);
                        }
                        else
                        {
                            dto.Tour = null;
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"Failed to load tour {response.TourId}: {ex.Message}");
                        dto.Tour = null;
                    }
                }
            }

            return dtos;
        }

        public TourRequestDto AcceptResponse(AcceptResponseDto dto, long touristId)
        {
            var tourRequest = _repository.Get(dto.TourRequestId);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized.");

            var response = _repository.GetResponse(dto.ResponseId);

            if (response.TourRequestId != dto.TourRequestId)
                throw new InvalidOperationException("Response does not belong to this tour request.");

            if (response.ResponseType == ResponseType.CustomProposal)
            {
                if (response.Status != ResponseStatus.Ready)
                {
                    throw new InvalidOperationException(
                        "This custom proposal is not ready yet. The author needs to finalize the tour first.");
                }

                if (!response.TourId.HasValue)
                {
                    throw new InvalidOperationException("Tour has not been created yet.");
                }

                var tour = _tourRepository.Get(response.TourId.Value);
                if (tour.Status != TourStatus.Published)
                {
                    throw new InvalidOperationException("Tour must be published before acceptance.");
                }
            }

            _repository.AcceptResponse(dto.ResponseId, dto.TourRequestId);

            var updated = _repository.Get(dto.TourRequestId);
            var result = _mapper.Map<TourRequestDto>(updated);
            result.ResponseCount = _repository.GetResponseCount(updated.Id);
            result.DaysUntilExpiration = updated.DaysUntilExpiration();

            return result;
        }

        public void DeclineResponse(long responseId, long touristId)
        {
            var response = _repository.GetResponse(responseId);
            var tourRequest = _repository.Get(response.TourRequestId);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to decline this response.");

            if (response.Status != ResponseStatus.Pending)
                throw new InvalidOperationException("This response has already been processed.");

            response.Reject();
            _repository.UpdateResponse(response);
        }

        public TourRequestDto ExpressInterest(long tourRequestId, long responseId, long touristId)
        {
            var tourRequest = _repository.Get(tourRequestId);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized.");

            var response = _repository.GetResponse(responseId);

            if (response.TourRequestId != tourRequestId)
                throw new InvalidOperationException("Response does not belong to this tour request.");

            if (response.ResponseType != ResponseType.CustomProposal)
                throw new InvalidOperationException("You can only express interest for custom proposals.");

            _repository.ExpressInterest(responseId);

            // TODO: Pošalji notifikaciju autoru
            // _notificationService.NotifyAuthor(response.AuthorId, "Tourist is interested in your proposal!");

            var updated = _repository.Get(tourRequestId);
            var result = _mapper.Map<TourRequestDto>(updated);
            result.ResponseCount = _repository.GetResponseCount(updated.Id);
            result.DaysUntilExpiration = updated.DaysUntilExpiration();

            return result;
        }
    }
}