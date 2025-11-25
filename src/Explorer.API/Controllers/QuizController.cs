using AutoMapper;
using System.Collections.Generic;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public;
using Explorer.Tours.Core.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;


namespace Explorer.API.Controllers
{
    //[Authorize(Roles = "Author")]
    [Authorize]
    [Route("api/quizzes")]
    [ApiController]
    public class QuizController : ControllerBase
    {
        private readonly IQuizService _quizService;

        public QuizController(IQuizService quizService)
        {
            _quizService = quizService;

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

        [HttpGet("my-quizzes")]
        [Authorize(Roles = "author")]
        public ActionResult<List<QuizDto>> GetAllMyQuizzes()
        {
            var authorId = GetUserIdFromToken();
            return Ok(_quizService.GetByAuthor(authorId));
        }

        [HttpGet]
        public ActionResult<List<QuizDto>> GetAll()
        {
            return Ok(_quizService.GetAll());
        }
      

        [HttpPost("{id}/submit-answers")]
        public ActionResult<QuizResultDto> SubmitAnswers([FromRoute] long id, [FromBody] QuizSolveDto answers)
        {
            return Ok(_quizService.SubmitAnswers(id, answers));
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


