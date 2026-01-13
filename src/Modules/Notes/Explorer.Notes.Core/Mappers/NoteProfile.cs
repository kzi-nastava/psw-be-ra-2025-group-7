using AutoMapper;
using Explorer.Notes.API.Dtos;
using Explorer.Notes.Core.Domain;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Explorer.Notes.Core.Mappers
{
    public class NoteProfile : Profile
    {
        public NoteProfile()
        {
            CreateMap<Note, NoteDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => (API.Dtos.NoteTypeDto)src.Type));

            CreateMap<NoteDto, Note>();
        }
    }
}