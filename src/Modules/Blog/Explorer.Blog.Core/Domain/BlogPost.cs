using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Blog.Core.Domain
{
    public class BlogPost : AggregateRoot
    {
        public long AuthorId { get; private set; }

        public string Title { get; private set; }

        public string Description { get; private set; }

        public DateTime CreatedAt { get; private set; }

        public DateTime? LastModifiedAt { get; private set; }

        public BlogStatus Status { get; private set; }

        public List<BlogImage> Images { get; private set; } = new();

        public List<BlogComment> Comments { get; private set; } = new();
        public List<BlogVote> Votes { get; private set; } = new();
        public int Score => Votes.Sum(v => v.Value);



        protected BlogPost() { }

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images = null)
        {
            if (authorId == 0)
                throw new ArgumentException("AuthorId must be a positive number.", nameof(authorId));


            AuthorId = authorId;
            CreatedAt = DateTime.UtcNow;
            Status = BlogStatus.Draft;

            SetTitle(title);
            SetDescription(description);

            if (images != null)
            {
                Images = images.ToList();
            }
        }


        // Izmena bloga dok je u pripremi-naslov, opis, slike
        public void EditDraft(string title, string description, IEnumerable<BlogImage>? images)
        {
            ClosedBlogCheck();
            EnsureDraft();

            SetTitle(title);
            SetDescription(description);

            Images.Clear();
            if (images != null)
            {
                Images.AddRange(images);
            }

            LastModifiedAt = DateTime.UtcNow;
        }

        //izmena samo opisa-kad je blog objavljen
        public void UpdatePublishedDescription(string description)
        {
            ClosedBlogCheck();

            EnsurePublished();

            SetDescription(description);
            LastModifiedAt = DateTime.UtcNow;
        }


        public void Publish()
        {
            EnsureDraft();
            Status = BlogStatus.Published;
        }


        public void Archive()
        {
            if (Status == BlogStatus.Archived || Status == BlogStatus.Closed)
                throw new InvalidOperationException("Blog is already read-only.");


            if (Status != BlogStatus.Published &&
                Status != BlogStatus.Active &&
                Status != BlogStatus.Famous)
            {
                throw new InvalidOperationException("Only published or promoted blogs can be archived.");
            }

            Status = BlogStatus.Archived;
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

            Title = title.Trim();
        }

        private void SetDescription(string description)
        {
            Description = description ?? string.Empty;
        }

        private void EnsureDraft()
        {
            if (Status != BlogStatus.Draft)
                throw new InvalidOperationException("Blog can be modified as draft only while in Draft status.");
        }

        private void EnsurePublished()
        {
            if (Status != BlogStatus.Published)
                throw new InvalidOperationException("Operation allowed only for published blogs.");
        }

        public void AddComment(BlogComment comment)
        {
            ClosedBlogCheck();

            if (Status == BlogStatus.Draft)
                throw new InvalidOperationException("Comments can only be added to published blogs.");

            if (Status == BlogStatus.Archived || Status == BlogStatus.Closed)
                throw new InvalidOperationException("Comments cannot be added to archived nor closed blogs.");

            Comments.Add(comment);
            RecalculatePopularityStatus();
        }

        private void RecalculatePopularityStatus()
        {
            var score = Votes.Sum(v => v.Value);
            var commentCount = GetCommentCount();

            // CLOSED 
            if (score < -10)
            {
                Status = BlogStatus.Closed;
                return;
            }

            // FAMOUS
            if (score > 5 && commentCount > 2) // 500, 30
            {
                Status = BlogStatus.Famous;
                return;
            }

            // ACTIVE
            if (score > 3 || commentCount > 3) //30,10
            {
                Status = BlogStatus.Active;
                return;
            }

            // ne diraj Draft i Archived
            if (Status == BlogStatus.Draft || Status == BlogStatus.Archived)
                return;

            // fallback
            Status = BlogStatus.Published;
        }

        //Helper metoda
        private BlogComment GetCommentOrThrow(long commentId)
        {
            var comment = Comments.FirstOrDefault(c => c.Id == commentId);
            if (comment == null)
                throw new InvalidOperationException("Comment does not exist.");

            return comment;
        }

        public void DeleteComment(long commentId, long userId)
        {
            ClosedBlogCheck();
            var comment = GetCommentOrThrow(commentId);

            if (comment.UserId != userId)
                throw new InvalidOperationException("Only the author of the comment can delete it.");

            var timePassed = DateTime.UtcNow - comment.CreatedAt;
            if (timePassed > TimeSpan.FromMinutes(15))
                throw new InvalidOperationException("Comment can only be deleted within 15 minutes of creation.");

            Comments.Remove(comment);

            RecalculatePopularityStatus();
        }

        public void EditComment(long commentId, long userId, string newText)
        {
            ClosedBlogCheck();
            var comment = GetCommentOrThrow(commentId);

            if (comment.UserId != userId)
                throw new InvalidOperationException("Only the author of the comment can edit it.");

            var timePassed = DateTime.UtcNow - comment.CreatedAt;
            if (timePassed > TimeSpan.FromMinutes(15))
                throw new InvalidOperationException("Comment can only be edited within 15 minutes of creation.");

            comment.EditText(newText);
        }


        public void Vote(long userId, int value)
        {
            ClosedBlogCheck();

            if (value != 1 && value != -1 && value != 0)
                throw new ArgumentException("Vote must be +1 or -1 or 0" );

            var existingVote = Votes.FirstOrDefault(v => v.UserId == userId);

            if (value == 0)
            {
                if (existingVote != null)
                {
                    Votes.Remove(existingVote);
                    RecalculatePopularityStatus();
                }
                return;
            }

            if (existingVote != null)
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

            RecalculatePopularityStatus();

        }


        //broj komentara
        public int GetCommentCount()
        {
            if (Comments == null)
                return 0;

            return Comments.Count;
        }

        // da li je blog zatvoren
        public void ClosedBlogCheck() 
        {
            if (Status == BlogStatus.Closed)
                throw new InvalidOperationException("Blog is closed and read-only.");

        }


    }
}