using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;

namespace Explorer.Tours.Core.Domain
{
    public enum ProblemCategory {Safety,Navigation,Equipment, Other}
    public enum ProblemPriority {Low, Medium, High }

    public class TourProblem :Entity
    {
        public int TourId { get; init; }
        public int TouristId { get; init; }
        public ProblemCategory Category { get; init; }
        public ProblemPriority Priority { get; init; }
        public string Description { get; init; }
        public DateTime TimeReported { get; init; }
      
        public TourProblem(int tourId, int touristId, ProblemCategory category, ProblemPriority priority, string description, DateTime timeReported)
        {
            if (tourId <= 0) throw new ArgumentException("Invalid TourId.");
            if (touristId < 0) throw new ArgumentException("Invalid TouristId.");
            if (string.IsNullOrWhiteSpace(description)) throw new ArgumentException("Invalid Description.");
            TourId = tourId;
            TouristId = touristId;
            Category = category;
            Priority = priority;
            Description = description;
            TimeReported = timeReported;
        }
    }
}
