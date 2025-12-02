using Explorer.Tours.API.Dtos;

namespace Explorer.Tours.API.Public.Shopping
{
    public interface IShoppingCartService
    {
        ShoppingCartDto GetByTouristId(long touristId);
        ShoppingCartDto AddToCart(long touristId, long tourId);
        ShoppingCartDto RemoveFromCart(long touristId, long orderItemId);
        void ClearCart(long touristId);
    }
}
