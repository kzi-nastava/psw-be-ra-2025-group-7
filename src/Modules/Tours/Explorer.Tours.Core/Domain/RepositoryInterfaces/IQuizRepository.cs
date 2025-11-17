using Explorer.BuildingBlocks.Core.UseCases;
using System.Collections.Generic;

namespace Explorer.Tours.Core.Domain.RepositoryInterfaces;

public interface IQuizRepository
{
    Quiz Get(long id);
    List<Quiz> GetByAuthor(long authorId);
    Quiz Create(Quiz quiz);
    Quiz Update(Quiz quiz);
    void Delete(long id);

    void AddQuestion(long quizId, Question question);
    void RemoveQuestion(long quizId, long questionId);
}