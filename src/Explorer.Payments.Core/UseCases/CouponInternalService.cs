using AutoMapper;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Internal;
using Explorer.Payments.Core.Domain;
using Explorer.Payments.Core.Domain.RepositoryInterfaces;

namespace Explorer.Payments.Core.UseCases;

public class CouponInternalService : ICouponInternalService
{
    private readonly ICouponRepository _couponRepository;
    private readonly IMapper _mapper;

    public CouponInternalService(ICouponRepository couponRepository, IMapper mapper)
    {
        _couponRepository = couponRepository;
        _mapper = mapper;
    }

    public CouponDto CreateUniversalCouponForAuthor(long authorId, int discountPercentage)
    {
        var coupon = Coupon.CreateUniversalIssuedBy(authorId, discountPercentage);

        while (_couponRepository.GetByCode(coupon.Code) != null)
            coupon = Coupon.CreateUniversalIssuedBy(authorId, discountPercentage);

        var created = _couponRepository.Create(coupon);
        return _mapper.Map<CouponDto>(created);
    }

    public CouponDto CreateUniversalCoupon(int discountPercentage)
    {
        // Ako baš mora da postoji zbog interfejsa,
        // odluči šta je issuer: npr. "system" user id ili baci exception.
        throw new NotSupportedException("Universal coupon must have issuer authorId.");
    }


}
