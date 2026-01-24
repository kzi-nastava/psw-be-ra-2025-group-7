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

               var query = _tourRequests.Where(tr =>
                tr.Status == TourRequestStatus.Open ||
                tr.Status == TourRequestStatus.InProgress
);


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


        /*  public void AcceptResponse(long responseId, long tourRequestId)
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
          }*/

        public void AcceptResponse(long responseId, long tourRequestId)
        {
            using var transaction = _dbContext.Database.BeginTransaction();

            try
            {
                var acceptedResponse = _responses.FirstOrDefault(r => r.Id == responseId);
                if (acceptedResponse == null)
                    throw new NotFoundException($"Response with id {responseId} not found.");

                if (acceptedResponse.TourRequestId != tourRequestId)
                    throw new InvalidOperationException("Response does not belong to this tour request.");

                acceptedResponse.Accept();

                var otherResponses = _responses
                    .Where(r => r.TourRequestId == tourRequestId
                             && r.Id != responseId
                             && r.Status == ResponseStatus.Pending)
                    .ToList();

                foreach (var response in otherResponses)
                {
                    response.Reject();
                }

                var request = _tourRequests.FirstOrDefault(tr => tr.Id == tourRequestId);
                if (request == null)
                    throw new NotFoundException($"Tour request with id {tourRequestId} not found.");

                request.MarkFulfilled();

                _dbContext.SaveChanges();

                transaction.Commit();
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new InvalidOperationException($"Error accepting response: {ex.Message}", ex);
            }
        }




        public PagedResult<TourRequest> GetOpenRequestsFiltered(int page, int pageSize, decimal? minBudget, decimal? maxBudget, int? difficulty)
        {
            var query = _tourRequests.Where(tr =>
                tr.Status == TourRequestStatus.Open || tr.Status == TourRequestStatus.InProgress
            );

            if (minBudget.HasValue) query = query.Where(tr => tr.Budget >= minBudget.Value);
            if (maxBudget.HasValue) query = query.Where(tr => tr.Budget <= maxBudget.Value);


            if (difficulty.HasValue)
            {
                var diffEnum = (TourDifficulty)difficulty.Value;
                query = query.Where(tr => tr.PreferredDifficulty == diffEnum);
            }


            var totalCount = query.Count();

            var items = query
                .OrderByDescending(tr => tr.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<TourRequest>(items, totalCount);
        }




        public List<TourRequestResponse> GetResponsesByAuthor(long authorId)
        {
            return _responses
                .AsNoTracking()
                .Where(r => r.AuthorId == authorId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();
        }


        public Tour GetForPreview(long id)
        {
            var tour = _dbContext.Tours
                .Include(t => t.KeyPoints)
                .Include(t => t.TourDurations)
                .Include(t => t.RequiredEquipment)
                .Include(t => t.Images)
                .AsNoTracking()  
                .FirstOrDefault(t => t.Id == id);

            if (tour == null)
            {
                throw new KeyNotFoundException($"Tour with id {id} not found.");
            }

            return tour;
        }

        public void ExpressInterest(long responseId)
        {
            var response = _dbContext.TourRequestResponses
                .FirstOrDefault(r => r.Id == responseId);

            if (response == null)
                throw new KeyNotFoundException($"Response {responseId} not found.");

            response.ExpressInterest();
            _dbContext.SaveChanges();
        }

        public void MarkResponseAsReady(long responseId)
        {
            var response = _dbContext.TourRequestResponses
                .FirstOrDefault(r => r.Id == responseId);

            if (response == null)
                throw new KeyNotFoundException($"Response {responseId} not found.");

            response.MarkAsReady();
            _dbContext.SaveChanges();
        }

    }
}