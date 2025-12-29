using System;
using System.Collections.Generic;

namespace Explorer.Notes.API.Dtos
{
    public class NoteDto
    {
        public long Id { get; set; }
        public long UserId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NoteTypeDto Type { get; set; }
        public List<string> Tags { get; set; } = new();
        public long? TourId { get; set; }
        public bool IsPinned { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}