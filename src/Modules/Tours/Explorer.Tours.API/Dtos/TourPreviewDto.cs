using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Explorer.Tours.API.Dtos;

/// <summary>
/// Preview of a tour visible to tourists before purchase.
/// Shows: description, duration, travel time, images (from first key point), starting point, and price.
/// Does NOT show: all key points, secrets.
/// </summary>
    public class TourPreviewDto
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; } = new();
    public decimal Price { get; set; }
    public decimal DiscountPercentage { get; set; }
    public bool IsPurchasable { get; set; }

    public decimal OriginalPrice { get; set; }
    public bool IsOnSale { get; set; }


    /// <summary>
    /// Duration information visible before purchase
    /// </summary>
    public List<TourDurationDto> TourDurations { get; set; } = new();
    
    /// <summary>
    /// Starting point (first key point) visible before purchase - without secret
    /// </summary>
    public KeyPointWithoutSecretDto? StartingPoint { get; set; }

    // TODO: compare usage to StartingPoint
    public KeyPointDto? FirstKeyPoint { get; set; }

    /// <summary>
    /// Image from the starting point
    /// </summary>
    public string? PreviewImageUrl => StartingPoint?.ImageUrl;
}
