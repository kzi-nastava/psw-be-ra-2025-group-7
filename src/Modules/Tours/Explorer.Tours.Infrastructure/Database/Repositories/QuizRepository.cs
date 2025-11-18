using Explorer.Tours.Core.Domain.Entities;
using Explorer.Tours.Core.Domain.RepositoryInterfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Tours.Infrastructure.Database;
using AutoMapper; // Adjust namespace if your ToursContext is elsewhere


namespace Explorer.Tours.Infrastructure.Database.Repositories;

public class QuizRepository : IQuizRepository

{
	private readonly ToursContext _context;
	private readonly IMapper _mapper;

    public QuizRepository(ToursContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public Quiz Get(long id)
	{
		var quiz = _context.Quizzes
			//.Include(q => q.Questions)
			//.ThenInclude(q => q.Options)
			.FirstOrDefault(q => q.Id == id);
		return quiz;
	}

	public Quiz Create(Quiz quiz)
	{
		_context.Quizzes.Add(quiz);
		_context.SaveChanges();
		return quiz;
	}

	public Quiz Update(Quiz quiz, long id)
	{
		var existing = _context.Quizzes.FirstOrDefault(x => x.Id == id);
		if (existing != null)
		{
			existing.Title = quiz.Title;
        }
		//_context.Quizzes.Update(quiz);
		_context.SaveChanges();
		return quiz;
    }

    public void AddQuestion(long quizId, Question question)
	{
		var quiz = _context.Quizzes
			.Include(q => q.Questions)
			.FirstOrDefault(q => q.Id == quizId);

		if (quiz == null)
		{
			throw new Exception("Quiz not found");
		}

        if (quiz.Questions.Count >= 5)
        {
            throw new InvalidOperationException("A quiz cannot have more than 5 questions.");
        }

        quiz.Questions.Add(question);
		_context.SaveChanges();
	}

	public void RemoveQuestion(long quizId, long questionId)
	{
		var quiz = _context.Quizzes
			.Include(q => q.Questions)
			.FirstOrDefault(q => q.Id == quizId);
		if (quiz == null)
		{
			throw new Exception("Quiz not found");
		}
		var question = quiz.Questions.FirstOrDefault(q => q.Id == questionId);
		if (question != null)
		{
			quiz.Questions.Remove(question);
			_context.SaveChanges();
		}
		else
		{
			throw new Exception("Question not found in the specified quiz");
		}
    }

    public void Delete(long id)
	{
		var quiz = _context.Quizzes.Find(id);
		if (quiz != null)
		{
			_context.Quizzes.Remove(quiz);
			_context.SaveChanges();
		}
		else 
		{
            throw new KeyNotFoundException($"Quiz with id {id} not found.");
        }
    }

    public List<Quiz> GetByAuthor(long authorId)
	{
		return _context.Quizzes
			.Include(q => q.Questions)
			.ThenInclude(q => q.Options)
			.Where(q => q.AuthorId == authorId)
			.ToList();
	}
}
