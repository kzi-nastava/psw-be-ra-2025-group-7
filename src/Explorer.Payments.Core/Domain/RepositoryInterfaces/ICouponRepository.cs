namespace Explorer.Payments.Core.Domain.RepositoryInterfaces
{
    public interface ICouponRepository
    {
        Coupon Get(long id);
        Coupon? GetByCode(string code);
        List<Coupon> GetByAuthor(long authorId);
        Coupon Create(Coupon coupon);
        Coupon Update(Coupon coupon);
        void Delete(long id);
    }
}
