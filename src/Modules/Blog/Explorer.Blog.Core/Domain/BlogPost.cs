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

        protected BlogPost() { }  

        public BlogPost(long authorId, string title, string description, IEnumerable<BlogImage>? images = null)
        {
            if (authorId <= 0)
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
    }
}