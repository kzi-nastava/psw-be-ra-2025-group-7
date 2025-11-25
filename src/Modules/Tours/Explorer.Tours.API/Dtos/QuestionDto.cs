namespace Explorer.Tours.API.Dtos;
using System.Collections.Generic;


public class QuestionDto
{
    public int Id { get; set; }
    public string Content { get; set; }
    public bool AllowsMultipleCorrect { get; set; }

    public List<OptionDto> Options { get; set; } = new();



}
