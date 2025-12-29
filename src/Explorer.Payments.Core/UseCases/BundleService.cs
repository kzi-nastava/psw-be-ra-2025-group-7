using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class BundleService : IBundleService
    {
        private readonly IBundleRepository _bundleRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public BundleService(IBundleRepository bundleRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _bundleRepository = bundleRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public List<BundleDto> GetByAuthor(long authorId)
        {
            var bundles = _bundleRepository.GetByAuthor(authorId);

            var result = new List<BundleDto>();
            foreach (var b in bundles)
            {
                var dto = _mapper.Map<BundleDto>(b);
                dto.ToursTotalPrice = CalculateAndValidateToursTotal(authorId, dto.TourIds);
                result.Add(dto);
            }

            return result;
        }

        public BundleDto Get(long id, long authorId)
        {
            var bundle = _bundleRepository.Get(id);
            EnsureOwner(bundle, authorId);

            var dto = _mapper.Map<BundleDto>(bundle);
            dto.ToursTotalPrice = CalculateAndValidateToursTotal(authorId, dto.TourIds);
            return dto;
        }

        public BundleDto Create(long authorId, CreateBundleDto dto)
        {
            ValidateDto(dto?.Name, dto?.TourIds, dto?.Price);

            // validate ownership + calculate total
            var toursTotal = CalculateAndValidateToursTotal(authorId, dto.TourIds);

            var bundle = new Bundle(authorId, dto.Name, dto.Price, dto.TourIds);
            var created = _bundleRepository.Create(bundle);

            var result = _mapper.Map<BundleDto>(created);
            result.ToursTotalPrice = toursTotal;
            return result;
        }

        public BundleDto Update(long id, long authorId, UpdateBundleDto dto)
        {
            ValidateDto(dto?.Name, dto?.TourIds, dto?.Price);

            var existing = _bundleRepository.Get(id);
            EnsureOwner(existing, authorId);

            var toursTotal = CalculateAndValidateToursTotal(authorId, dto.TourIds);

            existing.Update(dto.Name, dto.Price, dto.TourIds);
            var updated = _bundleRepository.Update(existing);

            var result = _mapper.Map<BundleDto>(updated);
            result.ToursTotalPrice = toursTotal;
            return result;
        }

        public void Delete(long id, long authorId)
        {
            var existing = _bundleRepository.Get(id);
            EnsureOwner(existing, authorId);

            _bundleRepository.Delete(id);
        }

        public BundlePreviewResponseDto PreviewTotal(long authorId, BundlePreviewRequestDto dto)
        {
            if (dto?.TourIds == null || dto.TourIds.Count == 0)
                throw new ArgumentException("You must select at least one tour.");

            var total = CalculateAndValidateToursTotal(authorId, dto.TourIds);
            return new BundlePreviewResponseDto { ToursTotalPrice = total };
        }

        private static void ValidateDto(string name, List<long> tourIds, decimal? price)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Bundle name is required.");

            if (name.Length > 200)
                throw new ArgumentException("Bundle name cannot exceed 200 characters.");

            if (price == null || price < 0)
                throw new ArgumentException("Bundle price cannot be negative.");

            if (tourIds == null || tourIds.Count == 0)
                throw new ArgumentException("Bundle must contain at least one tour.");
        }

        private decimal CalculateAndValidateToursTotal(long authorId, List<long> tourIds)
        {
            decimal total = 0;

            foreach (var tourId in tourIds.Distinct())
            {
                var tour = _tourRepository.Get(tourId);

                if (tour.AuthorId != authorId)
                    throw new InvalidOperationException($"Tour {tourId} does not belong to this author.");

                total += tour.Price;
            }

            return total;
        }

        private static void EnsureOwner(Bundle bundle, long authorId)
        {
            if (bundle == null) throw new NotFoundException("Bundle not found.");

            if (bundle.AuthorId != authorId)
                throw new InvalidOperationException("You are not allowed to access this bundle.");
        }
    }
}
