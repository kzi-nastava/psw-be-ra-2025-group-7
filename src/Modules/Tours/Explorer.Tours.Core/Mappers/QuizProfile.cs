using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain.Entities;

namespace Explorer.Tours.Core.Mappers;

public class QuizProfile : Profile
{
    public QuizProfile()
    {
        // read dtos
        CreateMap<Quiz, QuizDto>().ReverseMap();
        CreateMap<CreateQuizDto, Quiz>();
    
        CreateMap<Question, QuestionDto>().ReverseMap();
        CreateMap<CreateQuestionDto, Question>();
        //    .ForCtorParam("quizId", opt => opt.MapFrom((src, ctx) => (long)ctx.Items["quizId"]));

    //    CreateMap<Option, OptionDto>().ReverseMap();
    //    CreateMap<CreateOptionDto, Option>();
    }
}