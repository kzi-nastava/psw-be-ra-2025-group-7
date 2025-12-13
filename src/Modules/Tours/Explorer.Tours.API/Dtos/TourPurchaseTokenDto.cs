namespace Explorer.Tours.API.Dtos;

public class TourPurchaseTokenDto
{
    public long Id { get; set; }
    public long UserId { get; set; }
    public long TourId { get; set; }
    public DateTime PurchaseDate { get; set; }
    
    /// <summary>
    /// Full tour information available after purchase.
    /// Includes all key points (without secrets) that can be used for tour execution.
    /// </summary>
    public PurchasedTourInfoDto? Tour { get; set; }
}
