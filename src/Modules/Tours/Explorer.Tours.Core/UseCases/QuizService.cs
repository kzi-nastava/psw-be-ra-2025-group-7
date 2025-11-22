using System;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain.Entities;// Quiz, Question, Option
using Explorer.Tours.Core.Domain.RepositoryInterfaces; // IQuizRepository
using Explorer.Tours.API.Dtos; // QuizDto, QuestionDto, OptionDto
using AutoMapper; // IMapper
using Explorer.Tours.API.Public; // IQuizService

namespace Explorer.Tours.Core.UseCases.Administration
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

		public QuizDto Get(long id)
		{
			var quiz = _quizRepository.Get(id);
			if (quiz == null)
				throw new KeyNotFoundException($"Quiz with id {id} not found.");

			return _mapper.Map<QuizDto>(quiz); // mapira entitet u dto
        }

		public List<QuizDto> GetByAuthor(long authorId) 
		{ 
			var quizzes = _quizRepository.GetByAuthor(authorId);
			return _mapper.Map<List<QuizDto>>(quizzes); // mapira listu entiteta u dto
        }

		public QuizDto Create(CreateQuizDto quizDto) 
		{
            if (quizDto == null)
                throw new ArgumentNullException(nameof(quizDto));

            if (quizDto.Questions != null && quizDto.Questions.Count > 5)
                throw new InvalidOperationException("A quiz cannot have more than 5 questions.");

            var quiz = _mapper.Map<Quiz>(quizDto); // mapira dto u entitet
			var createdQuiz = _quizRepository.Create(quiz);


			return _mapper.Map<QuizDto>(createdQuiz); // mapira entitet u dto
        }

		public QuizDto Update(QuizDto quizDto, long id) 
		{ 
			if (quizDto == null)
				throw new ArgumentNullException(nameof(quizDto));
			var quiz = _mapper.Map<Quiz>(quizDto); // mapira dto u entitet
			var updatedQuiz = _quizRepository.Update(quiz, id);

			return _mapper.Map<QuizDto>(updatedQuiz); // mapira entitet u dto
        }

		public void Delete(long id) 
		{ 
			if ( _quizRepository.Get(id) == null)
				throw new KeyNotFoundException($"Quiz with id {id} not found.");
			
			_quizRepository.Delete(id);

        }

		public QuizDto AddQuestion(long quizId, CreateQuestionDto questionDto) 
		{
            if (questionDto == null)
				throw new ArgumentNullException(nameof(questionDto));

			var quiz = _quizRepository.Get(quizId);
			if (quiz == null) { throw new Exception("Quiz not found"); }	

			var question = _mapper.Map<Question>(questionDto); // mapira dto u entitet
          
            quiz.AddQuestion(question); // dodaje pitanje u kviz
            _quizRepository.Update(quiz, quizId); // azurira kviz u repozitorijumu

            return _mapper.Map<QuizDto>(quiz); // mapira entitet u dto

        }

		public QuizDto RemoveQuestion(long quizId, long questionId) 
		{ 
			if (_quizRepository.Get(quizId) == null)
				throw new KeyNotFoundException($"Quiz with id {quizId} not found.");

			_quizRepository.RemoveQuestion(quizId, questionId);

			var updatedQuiz = _quizRepository.Get(quizId);
			return _mapper.Map<QuizDto>(updatedQuiz);
        }

    }
}