namespace Explorer.Tours.API.Dtos
{
    public class CreateOptionDto
    {
        public string Text { get; set; }
        public bool IsCorrect { get; set; }
        public string Feedback { get; set; }
    }
}
