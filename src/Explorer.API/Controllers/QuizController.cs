using System.Collections.Generic;
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
    }
}


