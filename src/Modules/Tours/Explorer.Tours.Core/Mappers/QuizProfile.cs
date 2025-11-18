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
        CreateMap<Question, QuestionDto>().ReverseMap();
        CreateMap<Option, OptionDto>().ReverseMap();

        // create dtos
        CreateMap<CreateQuizDto, Quiz>();
        CreateMap<CreateQuestionDto, Question>();
        CreateMap<CreateOptionDto, Option>();
    }
}