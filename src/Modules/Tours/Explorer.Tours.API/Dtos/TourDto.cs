namespace Explorer.Tours.API.Dtos
{
    public class TourDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Difficulty { get; set; }
        public List<string> Tags { get; set; }
        public int Status { get; set; }
        public decimal Price { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? ArchivedAt { get; set; }
    }
}
