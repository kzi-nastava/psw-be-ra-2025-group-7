using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class BlogVote : Entity
    {
        public long UserId { get; private set; }
        public int Value { get; private set; } // +1 ili -1
        public DateTime VotedAt { get; private set; }
        public BlogVote() { }
        public BlogVote(long userId, int value)
        {
            if (value != 1 && value != -1)
                throw new ArgumentException("Value must be either +1 or -1.", nameof(value));
         
            UserId = userId;
            Value = value;
            VotedAt = DateTime.UtcNow;
        }

        public void ChangeVote(int newValue)
        {
            if (newValue != 1 && newValue != -1)
                throw new ArgumentException("Value must be either +1 or -1.", nameof(newValue));
            
            Value = newValue;
            VotedAt = DateTime.UtcNow;
        }
    }
}
