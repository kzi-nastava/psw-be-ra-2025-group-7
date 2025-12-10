using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.API.Dtos
{
    public class BlogVoteDto
    {
       // public long Id { get; set; }
        public long UserId { get; set; }
        public int Value { get; set; }
        public DateTime VotedAt { get; set; }

        public int Score { get; set; }
    }
}
