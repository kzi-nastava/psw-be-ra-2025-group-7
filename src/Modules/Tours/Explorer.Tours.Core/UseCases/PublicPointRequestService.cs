using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Tours.Core.UseCases
{
    public class PublicPointRequestService
    {
        private readonly IPublicPointRequestRepository _requestRepo;
        private readonly ITourRepository _tourRepo;

        public PublicPointRequestService(IPublicPointRequestRepository requestRepo, ITourRepository tourRepo)
        {
            _requestRepo = requestRepo;
            _tourRepo = tourRepo;
        }

        public PublicPointRequest Create(long tourId, int keyPointIndex, long authorId)
        {
            var tour = _tourRepo.Get(tourId);
            if (tour == null) throw new NotFoundException("Tour not found.");

            if (tour.AuthorId != authorId)
                throw new ForbiddenException("You are not the author of this tour.");

            if (keyPointIndex < 0 || keyPointIndex >= tour.KeyPoints.Count)
                throw new ArgumentException("Key point index is out of range.");

            if (tour.KeyPoints[keyPointIndex].IsPublic)
                throw new ArgumentException("Key point is already public.");

            var existing = _requestRepo.GetPending(tourId, keyPointIndex);
            if (existing != null)
                throw new ArgumentException("Public point request is already pending for this key point.");

            var req = new PublicPointRequest(tourId, keyPointIndex, authorId);
            return _requestRepo.Create(req);
        }
    }
}
