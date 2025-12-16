using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain;

public class TourPurchaseToken : Entity
{
    public long UserId { get; init; }
    public long TourId { get; init; }
    public DateTime PurchaseDate { get; init; }

    public Tour Tour { get; init; }

    // EF Core constructor
    private TourPurchaseToken() { }

    public TourPurchaseToken(long userId, long tourId)
    {
        UserId = userId;
        TourId = tourId;
        PurchaseDate = DateTime.UtcNow;
        Validate();
    }

    private void Validate()
    {
    }

    public static TourPurchaseToken CreateForTour(long userId, Tour tour)
    {
        if (tour == null)
            throw new ArgumentNullException(nameof(tour));

        // only published tours can be purchased
        tour.ValidatePurchase();

        return new TourPurchaseToken(userId, tour.Id);
    }
}
