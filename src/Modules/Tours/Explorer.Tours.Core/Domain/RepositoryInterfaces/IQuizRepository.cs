using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain.Entities;// Quiz, Question, Option


namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IQuizRepository
{
    Quiz Get(long id);
    List<Quiz> GetByAuthor(long authorId);
    Quiz Create(Quiz quiz);
    Quiz Update(Quiz quiz, long id);
    void Delete(long id); //brise kviz po id-u

    void AddQuestion(long quizId, Question question);
    void RemoveQuestion(long quizId, long questionId);
}