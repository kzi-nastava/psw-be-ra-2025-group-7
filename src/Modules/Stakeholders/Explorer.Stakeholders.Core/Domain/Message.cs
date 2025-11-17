using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Domain
{
    public class Message : Entity
    {
        public long SentByUserId { get; init; }
        public long SentToUserId { get; init; }
        public string Content { get; private set; }
        public DateTime SentAt { get; private set; }
        public DateTime EditedAt { get; private set; } = DateTime.MinValue;

        public Message(long sentByUserId, long sentToUserId, string content, DateTime sentAt)
        {
            SentByUserId = sentByUserId;
            SentToUserId = sentToUserId;
            Content = content;
            SentAt = sentAt;
            Validate();
        }

        private void Validate()
        {
            if (SentByUserId == 0) throw new ArgumentException("Invalid SentByUserId");
            if (SentToUserId == 0) throw new ArgumentException("Invalid SentToUserId");
            if (string.IsNullOrWhiteSpace(Content)) throw new ArgumentException("Invalid Content");
            if (SentAt == default) throw new ArgumentException("Invalid SentAt");
        }
    }
}
