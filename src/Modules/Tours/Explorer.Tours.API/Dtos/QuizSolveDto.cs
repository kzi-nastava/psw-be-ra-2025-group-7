namespace Explorer.Tours.API.Dtos
{
    public class QuizSolveDto
    {
        // Key = question ID, Value = list of selected option IDs
        public Dictionary<long, List<long>> SelectedAnswers { get; set; } = new();
    }
}
