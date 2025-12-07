using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Explorer.Tours.Core.Domain
{
    public class TourProblemMessage : ValueObject
    {
        public int CreatorId { get; private set; }
        public string Message { get; private set; }
        public DateTime CreatedAt { get; private set; }

        [JsonConstructor] 
        public TourProblemMessage(int creatorId, string message, DateTime createdAt)
        {
            CreatorId = creatorId;
            Message = message.Trim();
            CreatedAt = createdAt;
        }
        public TourProblemMessage(int creatorId, string message)
        {
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Invalid message.");

            CreatorId = creatorId;
            Message = message.Trim();
            CreatedAt = DateTime.UtcNow;
        }
        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return CreatorId;
            yield return Message;
            yield return CreatedAt;
        }
    }
}
