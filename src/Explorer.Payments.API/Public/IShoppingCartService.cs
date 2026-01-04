using Explorer.Payments.API.Dtos;

namespace Explorer.Payments.API.Public
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetByTouristId(long touristId);
        ShoppingCartDto AddToCart(long touristId, long tourId);
        ShoppingCartDto RemoveFromCart(long touristId, long orderItemId);
        void ClearCart(long touristId);
        List<object> PurchaseCart(long touristId);
        List<object> PurchaseCartWithCoupon(long touristId, string? couponCode);
    }
}
