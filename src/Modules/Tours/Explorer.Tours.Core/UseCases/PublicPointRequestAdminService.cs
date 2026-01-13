using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class PublicPointRequestAdminService
    {
        private readonly IPublicPointRequestRepository _requestRepository;
        private readonly ITourRepository _tourRepository;

        public PublicPointRequestAdminService(
            IPublicPointRequestRepository requestRepository,
            ITourRepository tourRepository)
        {
            _requestRepository = requestRepository;
            _tourRepository = tourRepository;
        }

        public void Approve(long requestId, string? comment = null)
        {
            var request = _requestRepository.Get(requestId)
                ?? throw new NotFoundException("Public point request not found.");

            request.Approve(comment);

            var tour = _tourRepository.Get(request.TourId)
                ?? throw new NotFoundException("Tour not found.");

            // 👇 metoda koju si već dodao u Tour
            tour.ApproveKeyPointAsPublic(request.KeyPointIndex);

            _tourRepository.Update(tour);
            _requestRepository.Update(request);
        }

        public void Reject(long requestId, string comment)
        {
            var request = _requestRepository.Get(requestId)
                ?? throw new NotFoundException("Public point request not found.");

            request.Reject(comment);

            _requestRepository.Update(request);
        }
    }
}
