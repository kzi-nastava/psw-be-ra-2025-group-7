using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : Entity
    {
        public long AuthorId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public List<BlogImage> Images { get; private set; } = new();

        public List<BlogVote> Votes { get; private set; } = new();
        public int Score => Votes.Sum(v => v.Value);



        protected BlogPost() { }  

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images = null)
        {
            if (authorId <= 0)
                throw new ArgumentException("AuthorId must be a positive number.", nameof(authorId));

            SetTitle(title);
            SetDescription(description);

            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;

            if (images != null)
            {
                Images = images.ToList();
            }
        }

        public void Edit(string title, string description, IEnumerable<BlogImage>? images)
        {
            SetTitle(title);
            SetDescription(description);

            Images.Clear();
            if (images != null)
            {
                Images.AddRange(images);
            }
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

            Title = title;
        }

        private void SetDescription(string description)
        {
            Description = description ?? string.Empty;
            // Renderovanje Markdown -> HTML radi API ili frontend.
        }

        public void Vote(long userId, int value)
        {
            if (value != 1 && value != -1)
                throw new ArgumentException("Vote must be +1 or -1");

            var existingVote = Votes.FirstOrDefault(v => v.UserId == userId);


            if (existingVote != null )
            {
                if (existingVote.Value == value) // alo je kliknuo isto onda povuci glas
                {
                    Votes.Remove(existingVote);
                    return;
                }

                existingVote.ChangeVote(value); //menja se glas
            }
            else
            {
                var newVote = new BlogVote(userId, value);  //prvi put se glasa
                Votes.Add(newVote);
            }


        }
    }

}
