using Explorer.BuildingBlocks.Core.UseCases;
using Explorer.Tours.API.Dtos;
using System.Collections.Generic;

namespace Explorer.Tours.API.Public
{

    public interface IQuizService
    {
        QuizDto Get(long id);
        List<QuizDto> GetByAuthor(long authorId);
        QuizDto Create(CreateQuizDto quizDto);
        QuizDto Update(QuizDto quizDto, long id);
        void Delete(long id);

        QuizDto AddQuestion(long quizId, CreateQuestionDto questionDto);
        QuizDto RemoveQuestion(long quizId, long questionId);
    }

}
