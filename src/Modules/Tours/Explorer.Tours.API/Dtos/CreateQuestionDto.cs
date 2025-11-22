namespace Explorer.Tours.API.Dtos;

using System.Collections.Generic;

public class CreateQuestionDto
{
    public string Content { get; set; }
    public bool AllowsMultipleCorrect { get; set; }
   // public List<CreateOptionDto> Options { get; set; } = new();
}