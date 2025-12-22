using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.Mappers
{
    public class EncountersProfile : Profile
    {
        public EncountersProfile()
        {
            CreateMap<Encounter, EncounterDto>()
                .ForMember(d => d.Latitude, o => o.MapFrom(s => s.Location.Latitude))
                .ForMember(d => d.Longitude, o => o.MapFrom(s => s.Location.Longitude))
                .ForMember(d => d.Radius, o => o.MapFrom(s => s.Location.Radius))
                .ForMember(d => d.Status, o => o.MapFrom(s => s.Status.ToString().ToLower()))
                .ForMember(d => d.Type, o => o.MapFrom(s => s.Type.ToString().ToLower()));

           
        }
    }

}
