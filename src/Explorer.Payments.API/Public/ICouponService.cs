using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface ICouponService
    {
        List<CouponDto> GetByAuthor(long authorId);
        CouponDto Get(long id, long authorId);
        CouponDto Create(long authorId, CreateCouponDto dto);
        CouponDto Update(long id, long authorId, UpdateCouponDto dto);
        void Delete(long id, long authorId);
        void Activate(long id, long authorId);
        void Deactivate(long id, long authorId);
        CouponDto? ValidateCoupon(string code);
    }
}
