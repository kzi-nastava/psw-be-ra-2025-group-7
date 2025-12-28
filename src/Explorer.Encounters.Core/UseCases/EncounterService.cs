using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterService : IEncounterService
    {
        private readonly IEncounterRepository _repo;
        private readonly IMapper _mapper;

        public EncounterService(IEncounterRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public EncounterDto Create(int creatorId,CreateEncounterDto dto)
        {
            var type = ParseType(dto.Type);
            var location = new GeoLocation(dto.Latitude, dto.Longitude, dto.Radius);
            if (dto.HiddenLocation != null && type != EncounterType.Location)
            {
                throw new InvalidOperationException(
                    "Hidden location is allowed only for Location encounters.");
            }
            var encounter = new Encounter(creatorId,dto.Name, dto.Description, location, dto.Xp, type, dto.RequiredParticipants); // default Draft
            

            
            // opcionalni HiddenLocation
            if (type == EncounterType.Location && dto.HiddenLocation != null)
            {
                var hl = dto.HiddenLocation;
                var hiddenConfig = new HiddenLocationEncounter(
                    new EncounterImage(hl.ImageUrl),
                    hl.ActivationRadiusMeters,
                    new GeoLocation(hl.PhotoLatitude, hl.PhotoLongitude)
                );
                encounter.SetHiddenLocationDetails(hiddenConfig);
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
                encounter.ChangeStatus(ParseStatus(dto.Status));

            _repo.Create(encounter);
            return _mapper.Map<EncounterDto>(encounter);
        }


        public EncounterDto Update(long id,int creatorId, UpdateEncounterDto dto)
        {
            var encounter = _repo.Get(id) ?? throw new KeyNotFoundException("Encounter not found.");

            var name = dto.Name ?? encounter.Name;
            var description = dto.Description ?? encounter.Description;
            var latitude = dto.Latitude ?? encounter.Location.Latitude;
            var longitude = dto.Longitude ?? encounter.Location.Longitude;
            var radius = dto.Radius ?? encounter.Location.Radius;
            var requiredParticipants = dto.RequiredParticipants ?? encounter.RequiredParticipants;
            var xp = dto.Xp ?? encounter.Xp;
            var type = string.IsNullOrWhiteSpace(dto.Type) ? encounter.Type : ParseType(dto.Type);
            GeoLocation location;

            if (type == EncounterType.Location)
            {
                location = new GeoLocation(latitude, longitude);
            }
            else
            {
                location = new GeoLocation(latitude, longitude, radius);
            }
            encounter.Update(creatorId, name, description, location, xp, type,requiredParticipants);
            if(type != EncounterType.Social)
            {
                requiredParticipants = null;
                radius = null;
            }
            
         

            if (type == EncounterType.Location && dto.HiddenLocation != null)
            {
                var hl = dto.HiddenLocation;
                var hiddenConfig = new HiddenLocationEncounter(
                    new EncounterImage(hl.ImageUrl),
                    hl.ActivationRadiusMeters,
                    new GeoLocation(hl.PhotoLatitude, hl.PhotoLongitude)
                );
                encounter.SetHiddenLocationDetails(hiddenConfig);
            }

            if (!string.IsNullOrWhiteSpace(dto.Status))
                encounter.ChangeStatus(ParseStatus(dto.Status));

            _repo.Update(encounter);
            return _mapper.Map<EncounterDto>(encounter);
        }



        public void Delete(long id, int creatorId)
        {
            var encounter = _repo.Get(id) ?? throw new KeyNotFoundException("Encounter not found.");

            if (encounter.CreatorId != creatorId)
                throw new InvalidOperationException("Only the creator can delete the encounter.");

            _repo.Delete(id);
        }


        public EncounterDto Get(long id)
        {
            var encounter = _repo.Get(id) ?? throw new KeyNotFoundException("Encounter not found.");
            return _mapper.Map<EncounterDto>(encounter);
        }

        public IEnumerable<EncounterDto> Get(string? status, string? type)
        {
            var s = string.IsNullOrWhiteSpace(status) ? (EncounterStatus?)null : ParseStatus(status);
            var t = string.IsNullOrWhiteSpace(type) ? (EncounterType?)null : ParseType(type);

            var page = _repo.GetPaged(1, 1000, s, t);
            return _mapper.Map<List<EncounterDto>>(page.Results);
        }

        public EncounterDto ChangeStatus(long id, string status)
        {
            var encounter = _repo.Get(id) ?? throw new KeyNotFoundException("Encounter not found.");
            encounter.ChangeStatus(ParseStatus(status));
            _repo.Update(encounter);
            return _mapper.Map<EncounterDto>(encounter);
        }

        private static EncounterStatus ParseStatus(string value)
        {
            return value.Trim().ToLower() switch
            {
                "draft" => EncounterStatus.Draft,
                "active" => EncounterStatus.Active,
                "archived" => EncounterStatus.Archived,
                _ => throw new ArgumentException("Invalid status.")
            };
        }

        private static EncounterType ParseType(string value)
        {
            return value.Trim().ToLower() switch
            {
                "social" => EncounterType.Social,
                "location" => EncounterType.Location,
                "misc" => EncounterType.Misc,
                _ => throw new ArgumentException("Invalid type.")
            };
        }

    }

}
