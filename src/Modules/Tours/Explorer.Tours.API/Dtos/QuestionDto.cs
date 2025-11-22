namespace Explorer.Tours.API.Dtos;
using System.Collections.Generic;


public class QuestionDto
{
    public string Content { get; set; }
    public bool AllowsMultipleCorrect { get; set; }

  //  ICollection<OptionDto> Options { get; set; } = new();

}
