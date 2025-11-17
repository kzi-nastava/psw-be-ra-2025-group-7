using System.Collections.Generic;

namespace Explorer.Tours.API.Dtos;

public class OptionDto
{
    public long Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; }
}

public class CreateOptionDto
{
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public string Feedback { get; set; }
}


