namespace Explorer.Tours.API.Dtos
{
    public class KeyPointDto
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        // 🔹 koristi autor (checkbox "make public")
        public bool MakePublic { get; set; }

        // 🔹 koristi turist / map search
        public bool IsPublic { get; set; }

        public string? ImageUrl { get; set; }

        // Tajna koja se otključava kada turista stigne do tačke
        public string Secret { get; set; } = string.Empty;
    }
}
