using System.Collections.Generic;
using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;


namespace Explorer.API.Controllers
{
    [Authorize(Roles = "Author")]
    [Route("api/quizzes")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;
        private readonly IMapper _mapper;

        public QuizController(IQuizService quizService, IMapper mapper)
        {
            _quizService = quizService;
            _mapper = mapper;

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
            quiz.AuthorId = GetUserIdFromToken();
            return Ok(_quizService.Create(quiz));
        }

        private long GetUserIdFromToken()
        {
            return long.Parse(User.FindFirst("id").Value);
        }

        [HttpPut("{id}")]
        public ActionResult<QuizDto> Update(long id, [FromBody] QuizDto quiz)
        {
            var existing = _quizService.Get(id);

            if (existing.AuthorId != GetUserIdFromToken())
                return Forbid(); // 403

            return Ok(_quizService.Update(quiz, id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(long id)
        {
            var existing = _quizService.Get(id);

            if (existing.AuthorId != GetUserIdFromToken())
                return Forbid();

            _quizService.Delete(id);
            return NoContent();
        }

        // question endpoints
        [HttpPost("{quizId}/questions")]
        public ActionResult<QuizDto> AddQuestion(long quizId, [FromBody] CreateQuestionDto question)
        {
            var quiz = _quizService.Get(quizId);

            if (quiz.AuthorId != GetUserIdFromToken())
                return Forbid();

            return Ok(_quizService.AddQuestion(quizId, question));
        }

        [HttpDelete("{quizId}/questions/{questionId}")]
        public ActionResult<QuizDto> RemoveQuestion(long quizId, long questionId)
        {
            var quiz = _quizService.Get(quizId);

            if (quiz.AuthorId != GetUserIdFromToken())
                return Forbid();

            return Ok(_quizService.RemoveQuestion(quizId, questionId));
        }


    }
}


