using AutoMapper;
using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases.Administration
{
    public class PublicPointRequestService : IPublicPointRequestService
    {
        private readonly IPublicPointRequestRepository _requestRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public PublicPointRequestService(
            IPublicPointRequestRepository requestRepository,
            ITourRepository tourRepository,
            IMapper mapper)
        {
            _requestRepository = requestRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public PagedResult<PublicPointRequestDto> GetPaged(int page, int pageSize)
        {
            var result = _requestRepository.GetPaged(page, pageSize);

            var dtos = result.Results.Select(request => EnrichRequestDto(request)).ToList();

            return new PagedResult<PublicPointRequestDto>(dtos, result.TotalCount);
        }

        public List<PublicPointRequestDto> GetAllPending()
        {
            var requests = _requestRepository.GetPendingRequests();

            return requests.Select(request => EnrichRequestDto(request)).ToList();
        }

        public PublicPointRequestDto ApproveRequest(long requestId, string? comment)
        {
            var request = _requestRepository.Get(requestId);

            // Poziva domensku metodu za odobravanje
            request.Approve(comment);

            var updatedRequest = _requestRepository.Update(request);

            // TODO: Implementirati slanje notifikacije preko Message sistema
            // Ovo može biti implementirano kao:
            // 1. Event-based system (Domain Events)
            // 2. Separat API call iz kontrolera
            // 3. Background job
            // Za sada samo logujemo
            Console.WriteLine($"[NOTIFICATION] Request {requestId} approved for author {request.AuthorId}. Comment: {comment ?? "N/A"}");

            return EnrichRequestDto(updatedRequest);
        }

        public PublicPointRequestDto RejectRequest(long requestId, string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment is required when rejecting a request.", nameof(comment));

            var request = _requestRepository.Get(requestId);

            // Poziva domensku metodu za odbijanje
            request.Reject(comment);

            var updatedRequest = _requestRepository.Update(request);

            // TODO: Implementirati slanje notifikacije preko Message sistema
            Console.WriteLine($"[NOTIFICATION] Request {requestId} rejected for author {request.AuthorId}. Reason: {comment}");

            return EnrichRequestDto(updatedRequest);
        }

        /// <summary>
        /// Obogaćuje DTO dodatnim informacijama iz ture (naziv ture i tačke)
        /// </summary>
        private PublicPointRequestDto EnrichRequestDto(PublicPointRequest request)
        {
            var dto = _mapper.Map<PublicPointRequestDto>(request);
            try
            {
                // ✅ Koristi repository metodu umesto direktnog DbContext pristupa
                var tour = _tourRepository.GetWithKeyPoints(request.TourId);

                if (tour != null)
                {
                    dto.TourName = tour.Name;

                    if (request.KeyPointIndex >= 0 && request.KeyPointIndex < tour.KeyPoints.Count)
                    {
                        dto.KeyPointName = tour.KeyPoints[request.KeyPointIndex].Name;
                    }
                }
            }
            catch
            {
                // Ako tura više ne postoji, ostavljamo null vrednosti
                dto.TourName = null;
                dto.KeyPointName = null;
            }
            return dto;
        }
    }
}