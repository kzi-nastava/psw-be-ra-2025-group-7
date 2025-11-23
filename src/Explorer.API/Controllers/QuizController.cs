using System.Collections.Generic;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;

namespace Explorer.API.Controllers
{
    [Route("api/quizzes")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IMapper _mapper;
        private IQuizService quizService;

        public QuizController(IQuizService quizService, IMapper mapper)
        {
            _quizService = quizService;
            _mapper = mapper;

        }

        public QuizController(IQuizService quizService)
        {
            this.quizService = quizService;
        }

        [HttpGet("{id}")]
        public ActionResult<QuizDto> Get(long id)
        {
            return Ok(_quizService.Get(id));
        }
        [HttpGet("author/{authorId}")]
        public ActionResult<List<QuizDto>> GetByAuthor(long authorId)
        {
            return Ok(_quizService.GetByAuthor(authorId));
        }

        [HttpPost]
        public ActionResult<QuizDto> Create([FromBody] CreateQuizDto quiz)
        {
            return Ok(_quizService.Create(quiz));
        }

        [HttpPut("{id}")]
        public ActionResult<QuizDto> Update(long id, [FromBody] QuizDto quiz)
        {
            return Ok(_quizService.Update(quiz, id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            _quizService.Delete(id);
            return NoContent();
        }

        // question endpoints
        [HttpPost("{quizId}/questions")]
        public ActionResult<QuizDto> AddQuestion(long quizId, [FromBody] CreateQuestionDto question)
        {
            if (question == null)
                return BadRequest("Question cannot be null.");

           // var questionDto = _mapper.Map<QuestionDto>(question);
            var updatedQuiz = _quizService.AddQuestion(quizId, question);
            return Ok(updatedQuiz);
        }

        [HttpDelete("{quizId}/questions/{questionId}")]
        public ActionResult<QuizDto> RemoveQuestion(long quizId, long questionId)
        {
            var updatedQuiz = _quizService.RemoveQuestion(quizId, questionId);
            return Ok(updatedQuiz);
        }

    }
}


