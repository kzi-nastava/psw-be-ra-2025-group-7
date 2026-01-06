using System.Collections.Generic;

namespace Explorer.Notes.API.Dtos
{
    public class UpdateNoteDto
    {
        public long Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NoteTypeDto Type { get; set; }
        public List<string> Tags { get; set; } = new();
        public long? TourId { get; set; }
    }
}