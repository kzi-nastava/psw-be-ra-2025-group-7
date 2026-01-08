using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
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

        public TourRequestService(ITourRequestRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
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
            return _mapper.Map<List<TourRequestResponseDto>>(responses);
        }

        public TourRequestDto AcceptResponse(AcceptResponseDto dto, long touristId)
        {
            var tourRequest = _repository.Get(dto.TourRequestId);

            if (tourRequest.TouristId != touristId)
                throw new UnauthorizedAccessException("You are not authorized to accept responses for this tour request.");

            if (tourRequest.Status == TourRequestStatus.Fulfilled)
                throw new InvalidOperationException("This offer has already been accepted.");

            if (tourRequest.Status == TourRequestStatus.Closed)
                throw new InvalidOperationException("This request is closed.");

            var response = _repository.GetResponse(dto.ResponseId);
            if (response.TourRequestId != dto.TourRequestId)
                throw new InvalidOperationException("Response does not belong to this tour request.");

            if (response.Status != ResponseStatus.Pending)
                throw new InvalidOperationException("This response has already been processed.");

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
    }
}