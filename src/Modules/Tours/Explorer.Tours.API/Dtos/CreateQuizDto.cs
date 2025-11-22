namespace Explorer.Tours.API.Dtos
{
    public class CreateQuizDto
    {
        public long AuthorId { get; set; }
        public string Title { get; set; }
        public List<CreateQuestionDto> Questions { get; set; } = new();

    }
}
