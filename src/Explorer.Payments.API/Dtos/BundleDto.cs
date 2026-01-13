namespace Explorer.Payments.API.Dtos
{
    public class BundleDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }

        public string Name { get; set; }
        public decimal Price { get; set; }

        // 0 = Draft
        public int Status { get; set; }

        public List<long> TourIds { get; set; } = new();

        // pomocni podatak za UI: suma cena izabranih tura
        public decimal ToursTotalPrice { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
