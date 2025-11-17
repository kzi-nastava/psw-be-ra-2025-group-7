using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class QuestionDto
{
    public long Id { get; set; }
    public string Content { get; set; }
    public bool AllowsMultipleCorrect { get; set; }

    public List<OptionDto> Options { get; set; } = new();
}

public class CreateQuestionDto
{
    public string Content { get; set; }
    public bool AllowsMultipleCorrect { get; set; }
    public List<CreateOptionDto> Options { get; set; } = new();
}

