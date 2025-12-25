using System.Collections.Generic;

namespace Explorer.Notes.API.Dtos
{
    public class CreateNoteDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public NoteTypeDto Type { get; set; }
        public List<string> Tags { get; set; } = new();
    }
}