using Explorer.BuildingBlocks.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Notes.Core.Domain
{
    public class Note : Entity
    {
        public long UserId { get; private set; }
        public string Title { get; private set; }
        public string Content { get; private set; }
        public NoteType Type { get; private set; }
        public List<string> Tags { get; private set; } = new();
        public long? TourId { get; private set; }
        public bool IsPinned { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime UpdatedAt { get; private set; }

        protected Note() { }

        public Note(long userId, string title, string content, NoteType type, List<string>? tags = null)
        {
            if (userId == 0)
                throw new ArgumentException("UserId not valid.", nameof(userId));

            UserId = userId;
            IsPinned = false;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;

            SetTitle(title);
            SetContent(content);
            SetType(type);
            SetTags(tags ?? new List<string>());
        }

        public void Update(string title, string content, NoteType type, List<string> tags)
        {
            SetTitle(title);
            SetContent(content);
            SetType(type);
            SetTags(tags);
            UpdatedAt = DateTime.UtcNow;
        }

        private void SetTitle(string title)
        {
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title cannot be empty.", nameof(title));

            if (title.Length > 200)
                throw new ArgumentException("Title cannot exceed 200 characters.", nameof(title));

            Title = title.Trim();
        }

        private void SetContent(string content)
        {
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty.", nameof(content));

            if (content.Length > 5000)
                throw new ArgumentException("Content cannot exceed 5000 characters.", nameof(content));

            Content = content.Trim();
        }

        private void SetType(NoteType type)
        {
            if (!Enum.IsDefined(typeof(NoteType), type))
                throw new ArgumentException("Invalid note type.", nameof(type));

            Type = type;
        }

        private void SetTags(List<string> tags)
        {
            Tags = tags.Select(t => t.Trim()).Where(t => !string.IsNullOrWhiteSpace(t)).ToList();
        }
    }
}