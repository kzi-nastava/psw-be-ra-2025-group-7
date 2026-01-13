namespace Explorer.Payments.API.Dtos
{
    public class UpdateBundleDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public List<long> TourIds { get; set; } = new();
    }
}
