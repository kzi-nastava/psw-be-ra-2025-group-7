namespace Explorer.Tours.API.Dtos;


public class QuizDto
{
    public string Title { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();

}
