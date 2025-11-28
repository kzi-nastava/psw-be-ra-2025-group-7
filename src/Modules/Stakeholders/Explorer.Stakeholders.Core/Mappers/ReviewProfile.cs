using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.Core.Mappers
{
    public class ReviewProfile : Profile
    {
        /*public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>();
        }*/

        public ReviewProfile()
        {
            CreateMap<Review, ReviewDto>()
                .ForMember(
                    dest => dest.PersonFirstName,
                    opt => opt.MapFrom(src => src.Person.Name))
                .ForMember(
                    dest => dest.PersonLastName,
                    opt => opt.MapFrom(src => src.Person.Surname));
        }
    }

}
