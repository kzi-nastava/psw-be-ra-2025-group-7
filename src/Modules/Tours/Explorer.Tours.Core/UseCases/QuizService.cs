using System;
using System.Collections.Generic;
namespace Explorer.Tours.Core.UseCases
{
	public class QuizService : IQuizService
	{
		private readonly IQuizRepository _quizRepository;
		private readonly IMapper _mapper;

        public QuizService(IQuizRepository quizRepository, IMapper mapper)
		{
			_quizRepository = quizRepository ?? throw new ArgumentNullException(nameof(quizRepository));
			_mapper = mapper;
        }

		public Quiz GetQuiz(long id)
		{
			var quiz = _quizRepository.Get(id);
			if (quiz == null)
				throw new KeyNotFoundException($"Quiz with id {id} not found.");

			return _mapper.Map<QuizDto>(quiz); // mapira entitet u dto
        }

		public List<Quiz> GetByAuthor(long authorId) 
		{ 
			var quizzes = _quizRepository.GetByAuthor(authorId);
			return _mapper.Map<List<QuizDto>>(quizzes); // mapira lisu entitet u dto
        }

		public Quiz CreateQuiz(QuizDto quizDto) 
		{
            if (quizDto == null)
                throw new ArgumentNullException(nameof(quizDto));

            var quiz = _mapper.Map<Quiz>(quizDto); // mapira dto u entitet
			var createdQuiz = _quizRepository.Create(quiz);


			return _mapper.Map<QuizDto>(createdQuiz); // mapira entitet u dto
        }

		public Quiz UpdateQuiz(QuizDto quizDto) 
		{ 
			if (quizDto == null)
				throw new ArgumentNullException(nameof(quizDto));
			var quiz = _mapper.Map<Quiz>(quizDto); // mapira dto u entitet
			var updatedQuiz = _quizRepository.Update(quiz);

			return _mapper.Map<QuizDto>(updatedQuiz); // mapira entitet u dto
        }

		public void DeleteQuiz(long id) 
		{ 
			if ( _quizRepository.Get(id) == null)
				throw new KeyNotFoundException($"Quiz with id {id} not found.");
			
			_quizRepository.Delete(id);

        }

		public Quiz AddQuestion(long quizId, QuestionDto questionDto) 
		{ 
			if (questionDto == null)
				throw new ArgumentNullException(nameof(questionDto));

			var question = _mapper.Map<Question>(questionDto); // mapira dto u entitet
			_quizRepository.AddQuestion(quizId, question);

            var updatedQuiz = _quizRepository.Get(quizId);
            return _mapper.Map<QuizDto>(updatedQuiz);

        }

		public Quiz RemoveQuestion(long quizId, long questionId) 
		{ 
			if (_quizRepository.Get(quizId) == null)
				throw new KeyNotFoundException($"Quiz with id {quizId} not found.");

			_quizRepository.RemoveQuestion(quizId, questionId);

			var updatedQuiz = _quizRepository.Get(quizId);
			return _mapper.Map<QuizDto>(updatedQuiz);
        }

    }
}


