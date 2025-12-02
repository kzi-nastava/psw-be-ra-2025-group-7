using AutoMapper;
using Explorer.Blog.API.Dtos;
using Explorer.Blog.Core.Domain;

namespace Explorer.Blog.Core.Mappers;

public class BlogProfile : Profile
{
    public BlogProfile()
    {
        
        CreateMap<BlogImage, BlogImageDto>().ReverseMap();

        
        CreateMap<BlogPost, BlogPostDto>()
            .ForMember(d => d.Images, opt => opt.MapFrom(src => src.Images))
            .ForMember(d => d.Status, opt => opt.MapFrom(src => src.Status.ToString()))
            .ForMember(d => d.Score, opt => opt.MapFrom(src => src.Score));


        CreateMap<CreateBlogPostDto, IEnumerable<BlogImage>>()
            .ConvertUsing(src => src.Images.Select(i => new BlogImage(i.Url, i.Order)));

        
        CreateMap<UpdateBlogPostDto, IEnumerable<BlogImage>>()
            .ConvertUsing(src => src.Images.Select(i => new BlogImage(i.Url, i.Order)));

        CreateMap<BlogVote, BlogVoteDto>().ReverseMap();
            


       

    }
}