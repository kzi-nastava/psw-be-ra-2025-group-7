using AutoMapper;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases
{
    public class CouponService : ICouponService
    {
        private readonly ICouponRepository _couponRepository;
        private readonly ITourRepository _tourRepository;
        private readonly IMapper _mapper;

        public CouponService(ICouponRepository couponRepository, ITourRepository tourRepository, IMapper mapper)
        {
            _couponRepository = couponRepository;
            _tourRepository = tourRepository;
            _mapper = mapper;
        }

        public List<CouponDto> GetByAuthor(long authorId)
        {
            var coupons = _couponRepository.GetByAuthor(authorId);
            return _mapper.Map<List<CouponDto>>(coupons);
        }

        public CouponDto Get(long id, long authorId)
        {
            var coupon = _couponRepository.Get(id);

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only view your own coupons.");

            return _mapper.Map<CouponDto>(coupon);
        }

        public CouponDto Create(long authorId, CreateCouponDto dto)
        {
            if (dto.TourId.HasValue)
            {
                var tour = _tourRepository.Get(dto.TourId.Value);
                if (tour.AuthorId != authorId)
                    throw new InvalidOperationException("You can only create coupons for your own tours.");
            }

            var coupon = new Coupon(authorId, dto.DiscountPercentage, dto.ExpirationDate, dto.TourId);

            // Ensure unique code
            while (_couponRepository.GetByCode(coupon.Code) != null)
            {
                coupon = new Coupon(authorId, dto.DiscountPercentage, dto.ExpirationDate, dto.TourId);
            }

            var created = _couponRepository.Create(coupon);
            return _mapper.Map<CouponDto>(created);
        }

        public CouponDto Update(long id, long authorId, UpdateCouponDto dto)
        {
            var coupon = _couponRepository.Get(id);

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only update your own coupons.");

            if (dto.TourId.HasValue)
            {
                var tour = _tourRepository.Get(dto.TourId.Value);
                if (tour.AuthorId != authorId)
                    throw new InvalidOperationException("You can only assign coupons to your own tours.");
            }

            coupon.Update(dto.DiscountPercentage, dto.ExpirationDate, dto.TourId);

            var updated = _couponRepository.Update(coupon);
            return _mapper.Map<CouponDto>(updated);
        }

        public void Delete(long id, long authorId)
        {
            var coupon = _couponRepository.Get(id);

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only delete your own coupons.");

            _couponRepository.Delete(id);
        }

        public void Activate(long id, long authorId)
        {
            var coupon = _couponRepository.Get(id);

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only activate your own coupons.");

            coupon.Activate();
            _couponRepository.Update(coupon);
        }

        public void Deactivate(long id, long authorId)
        {
            var coupon = _couponRepository.Get(id);

            if (coupon.AuthorId != authorId)
                throw new InvalidOperationException("You can only deactivate your own coupons.");

            coupon.Deactivate();
            _couponRepository.Update(coupon);
        }

        public CouponDto? ValidateCoupon(string code)
        {
            var coupon = _couponRepository.GetByCode(code);

            if (coupon == null || !coupon.IsValid())
                return null;

            return _mapper.Map<CouponDto>(coupon);
        }
    }
}
