namespace Explorer.Tours.API.Dtos;

public class OptionDto
{
    public int Id { get; set; }
    public string Text { get; set; }
    public bool IsCorrect { get; set; }
    public bool IsSelected { get; set; }
    public string Feedback { get; set; }
}



