namespace Explorer.Tours.API.Dtos;


public class QuizDto
{
    public long AuthorId { get; set; }
    public string Title { get; set; }
    public List<QuestionDto> Questions { get; set; } = new();

}
