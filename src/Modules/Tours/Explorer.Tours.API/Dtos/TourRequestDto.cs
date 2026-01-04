using System;

namespace Explorer.Tours.API.Dtos
{
    public class TourRequestDto
    {
        public long Id { get; set; }
        public long TouristId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Radius { get; set; }

        public decimal Budget { get; set; }

        public int? PreferredDifficulty { get; set; }
        public int NumberOfParticipants { get; set; }
        public DateTime? PreferredDate { get; set; }

        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }

        public int ResponseCount { get; set; }
        public int DaysUntilExpiration { get; set; }
    }

    public class CreateTourRequestDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Radius { get; set; }

        public int? PreferredDifficulty { get; set; }
        public int NumberOfParticipants { get; set; } = 1;
        public DateTime? PreferredDate { get; set; }
    }

    public class UpdateTourRequestDto
    {
        public long Id { get; set; }

        public string Title { get; set; }
        public string Description { get; set; }
        public decimal Budget { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public int? Radius { get; set; }

        public int? PreferredDifficulty { get; set; }
        public int NumberOfParticipants { get; set; }
        public DateTime? PreferredDate { get; set; }
    }


    public class TourRequestResponseDto
    {
        public long Id { get; set; }
        public long TourRequestId { get; set; }
        public long AuthorId { get; set; }
        public int ResponseType { get; set; }

        public long? TourId { get; set; }
        public string ProposalDescription { get; set; }

        public decimal ProposedPrice { get; set; }
        public string Message { get; set; } 
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public double? AuthorRating { get; set; }
        public TourPreviewDto Tour { get; set; }
    }

    public class TourRequestResponsePreviewDto
    {
        public long Id { get; set; }
        public long AuthorId { get; set; }
        public string AuthorName { get; set; }
        public string AuthorAvatar { get; set; }
        public double? AuthorRating { get; set; }

        public int ResponseType { get; set; }
        public decimal ProposedPrice { get; set; }
        public int Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class AcceptResponseDto
    {
        public long TourRequestId { get; set; }
        public long ResponseId { get; set; }
    }
}