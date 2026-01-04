using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Tours.Infrastructure.Database.Repositories
{
    public class TourRequestDbRepository : ITourRequestRepository
    {
        private readonly ToursContext _dbContext;
        private readonly DbSet<TourRequest> _tourRequests;
        private readonly DbSet<TourRequestResponse> _responses;

        public TourRequestDbRepository(ToursContext dbContext)
        {
            _dbContext = dbContext;
            _tourRequests = _dbContext.Set<TourRequest>();
            _responses = _dbContext.Set<TourRequestResponse>();
        }


        public TourRequest Get(long id)
        {
            var entity = _tourRequests.FirstOrDefault(tr => tr.Id == id);
            if (entity == null)
                throw new NotFoundException($"Tour request with id {id} not found.");
            return entity;
        }

        public TourRequest Create(TourRequest tourRequest)
        {
            _tourRequests.Add(tourRequest);
            _dbContext.SaveChanges();
            return tourRequest;
        }

        public TourRequest Update(TourRequest tourRequest)
        {
            try
            {
                var existing = _tourRequests.FirstOrDefault(tr => tr.Id == tourRequest.Id);
                if (existing == null)
                    throw new NotFoundException($"Tour request with id {tourRequest.Id} not found.");

                _dbContext.Entry(existing).CurrentValues.SetValues(tourRequest);
                _dbContext.SaveChanges();

                return Get(tourRequest.Id);
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException($"Error updating tour request: {e.InnerException?.Message ?? e.Message}");
            }
        }

        public void Delete(long id)
        {
            var entity = Get(id);

            var responses = _responses.Where(r => r.TourRequestId == id).ToList();
            _responses.RemoveRange(responses);

            _tourRequests.Remove(entity);
            _dbContext.SaveChanges();
        }

        public PagedResult<TourRequest> GetPagedByTourist(int page, int pageSize, long touristId)
        {
            var query = _tourRequests.Where(tr => tr.TouristId == touristId);

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(tr => tr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<TourRequest>(items, totalCount);
        }

        public PagedResult<TourRequest> GetOpenRequests(int page, int pageSize)
        {
            var query = _tourRequests.Where(tr => tr.Status == TourRequestStatus.Open);

            var totalCount = query.Count();

            var items = query
                .OrderByDescending(tr => tr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<TourRequest>(items, totalCount);
        }

        public List<TourRequest> GetExpiringSoon(int daysThreshold)
        {
            var thresholdDate = DateTime.UtcNow.AddDays(daysThreshold);

            return _tourRequests
                .Where(tr => tr.Status == TourRequestStatus.Open
                          && tr.ExpiresAt <= thresholdDate
                          && tr.ExpiresAt > DateTime.UtcNow)
                .ToList();
        }

        public int GetRequestCountForToday(long touristId)
        {
            var today = DateTime.UtcNow.Date;
            var tomorrow = today.AddDays(1);

            return _tourRequests
                .Count(tr => tr.TouristId == touristId
                          && tr.CreatedAt >= today
                          && tr.CreatedAt < tomorrow);
        }

        public TourRequestResponse GetResponse(long id)
        {
            var entity = _responses.FirstOrDefault(r => r.Id == id);
            if (entity == null)
                throw new NotFoundException($"Response with id {id} not found.");
            return entity;
        }

        public TourRequestResponse CreateResponse(TourRequestResponse response)
        {
            _responses.Add(response);
            _dbContext.SaveChanges();
            return response;
        }

        public TourRequestResponse UpdateResponse(TourRequestResponse response)
        {
            try
            {
                var existing = _responses.FirstOrDefault(r => r.Id == response.Id);
                if (existing == null)
                    throw new NotFoundException($"Response with id {response.Id} not found.");

                _dbContext.Entry(existing).CurrentValues.SetValues(response);
                _dbContext.SaveChanges();

                return GetResponse(response.Id);
            }
            catch (DbUpdateException e)
            {
                throw new NotFoundException($"Error updating response: {e.InnerException?.Message ?? e.Message}");
            }
        }

        public void DeleteResponse(long id)
        {
            var entity = GetResponse(id);
            _responses.Remove(entity);
            _dbContext.SaveChanges();
        }


        public List<TourRequestResponse> GetResponsesByRequest(long tourRequestId)
        {
            return _responses
                .Where(r => r.TourRequestId == tourRequestId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }

        public TourRequestResponse GetResponseByAuthorAndRequest(long authorId, long tourRequestId)
        {
            return _responses
                .FirstOrDefault(r => r.AuthorId == authorId && r.TourRequestId == tourRequestId);
        }

        public int GetResponseCount(long tourRequestId)
        {
            return _responses.Count(r => r.TourRequestId == tourRequestId);
        }


        public void AcceptResponse(long responseId, long tourRequestId)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var acceptedResponse = GetResponse(responseId);
                acceptedResponse.Accept();

                _dbContext.Entry(acceptedResponse).State = EntityState.Modified;
                _dbContext.SaveChanges();

                var otherResponses = _responses
                    .Where(r => r.TourRequestId == tourRequestId
                             && r.Id != responseId
                             && r.Status == ResponseStatus.Pending)
                    .ToList();

                foreach (var response in otherResponses)
                {
                    response.Reject();
                    _dbContext.Entry(response).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();

                var request = Get(tourRequestId);
                request.MarkFulfilled();

                _dbContext.Entry(request).State = EntityState.Modified;
                _dbContext.SaveChanges();

                transaction.Commit();
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }
    }
}