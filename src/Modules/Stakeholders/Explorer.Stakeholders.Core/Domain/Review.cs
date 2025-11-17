using Explorer.BuildingBlocks.Core.Domain;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Review : Entity
    {
        public int ReviewId { get; private set; }
        public int Rating { get; private set; }
        public string Comment { get; private set; }
        public long PersonId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? UpdatedAt { get; private set; }

        public Review(int reviewId, int rating, string comment, long personId)
        {
            ReviewId = reviewId;
            Rating = rating;
            Comment = comment;
            PersonId = personId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = null;
            Validate();
        }

        public void Update(int rating, string comment)
        {
            Rating = rating;
            Comment = comment;
            UpdatedAt = DateTime.UtcNow;
            Validate();
        }

        public void Validate()
        {
            if (Rating < 1 || Rating > 5) throw new ArgumentException("Invalid Rating");
            if (PersonId == 0) throw new ArgumentException("Invalid PersonId");
        }
    }
}
