using AutoMapper;
using Explorer.Encounters.API.Internal;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Encounters.API.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterInternalService : IEncounterInternalService
    {
        private readonly IEncounterRepository _repo;
        private readonly IMapper _mapper;

        public EncounterInternalService(IEncounterRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public IEnumerable<EncounterDto> GetByLocation(double Latitude, double Longitude)
        {
            var allEncounter = _repo.GetAll() ?? throw new KeyNotFoundException("Encounter not found.");
            List<EncounterDto> encounters = new List<EncounterDto>();
            foreach (var encounter in allEncounter)
            {
                if(GeoDistanceCalculator.IsWithinRadius(Latitude, Longitude, 500, encounter.Location.Latitude, encounter.Location.Longitude))
                {
                    encounters.Add(_mapper.Map<EncounterDto>(encounter));
                }
            }
            return encounters;
        }


    }
}
