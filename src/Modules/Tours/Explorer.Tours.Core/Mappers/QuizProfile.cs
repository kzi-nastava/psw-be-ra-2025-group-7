using AutoMapper;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.Core.Domain;

namespace Explorer.Tours.Core.Mappers;

public class QuizProfile : Profile
{
    public QuizProfile()
    {
        // read dtos
        CreateMap<QuizDto, Quiz>().ReverseMap();
        CreateMap<QuestionDto, Question>().ReverseMap();
        CreateMap<OptionDto, Option>().ReverseMap();

        // create dtos
        CreateMap<CreateQuizDto, Quiz>();
        CreateMap<CreateQuestionDto, Question>();
        CreateMap<CreateOptionDto, Option>();
    }
}