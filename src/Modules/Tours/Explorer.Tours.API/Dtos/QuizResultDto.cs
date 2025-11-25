using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.API.Dtos
{
    public class QuizResultDto
    {
        public int CorrectCount { get; set; }
        public int TotalQuestions { get; set; }
        public List<QuestionFeedbackDto> Feedback { get; set; }
    }
}
