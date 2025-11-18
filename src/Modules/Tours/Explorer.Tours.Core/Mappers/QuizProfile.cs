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
        //.ConstructUsing(src => new Quiz
        //{
        //    AuthorId = src.AuthorId,
        //    Title = src.Title
        //});



        //CreateMap<CreateQuizDto, Quiz>().ForMember(x => x.Questions, opt => opt.Ignore());



        //CreateMap<Question, QuestionDto>().ReverseMap();
        //CreateMap<Option, OptionDto>().ReverseMap();

        // create dtos
        //CreateMap<CreateQuestionDto, Question>();
        //CreateMap<CreateOptionDto, Option>();
    }
}