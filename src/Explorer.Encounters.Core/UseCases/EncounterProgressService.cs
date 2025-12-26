using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Explorer.Encounters.Core.Domain.EncounterProgress;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterProgressService : IEncounterProgressService
    {
        private readonly IEncounterProgressRepository _repo;
        private readonly IEncounterRepository _encounterRepo;
        private readonly IMapper _mapper;
        private readonly IUserProfileLocationService _userProfileLocService;

        public EncounterProgressService(IEncounterProgressRepository repo, IMapper mapper, IEncounterRepository encounterRepo, IUserProfileLocationService userProfileLocService)
        {
            _repo = repo;
            _mapper = mapper;
            _encounterRepo = encounterRepo;
            _userProfileLocService = userProfileLocService;

        }

        public EncounterProgressDto Create(EncounterProgressDto dto)
        {
            var status = EncounterProgressStatus.Active;
            var encounterProgress = new EncounterProgress(dto.EncounterId, dto.UserId, status);
            _repo.Create(encounterProgress);
            return _mapper.Map<EncounterProgressDto>(encounterProgress);
        }

        public EncounterDto Update(long id)
        {
            var encounterProgress = _repo.Get(id)
                ?? throw new KeyNotFoundException("EncounterProgress not found.");

            encounterProgress.SetCompleted();

            _repo.Update(encounterProgress);
            return _mapper.Map<EncounterDto>(encounterProgress);
        }
        public void Delete(long id) => _repo.Delete(id);

        public bool CheckEncounterProgress(long encounterId) { 
            var encounterProgresses = _repo.GetAll()
                .Where(ep => ep.EncounterId == encounterId && ep.Status == EncounterProgressStatus.Active)
                .ToList();

            var encounter = _encounterRepo.Get(encounterId);
            if(encounterProgresses.Count() < encounter.RequiredParticipants)
            {
                return false;
            }

            var usersInRadius = CheckParticipantsLocation(encounterProgresses, encounter);

            if (usersInRadius.Count >= encounter.RequiredParticipants)
            {
                FinishEncounterProgress(encounterProgresses, usersInRadius);
                return true;
            }

            return false;

        }

        public List<int> CheckParticipantsLocation(List<EncounterProgress> encounterProgresses, Encounter encounter)
        {
            var usersInRadius = new List<int>();

            foreach (var ep in encounterProgresses)
            {
                var user = _userProfileLocService.GetLocation(ep.UserId);

                if (user.Latitude == null || user.Longitude == null)
                    continue;

                bool isInside = GeoDistanceCalculator.IsWithinRadius(
                    encounter.Location.Latitude,
                    encounter.Location.Longitude,
                    encounter.Location.Radius ?? 0,
                    user.Latitude ?? 0,
                    user.Longitude ?? 0
                );

                if (isInside)
                {
                    usersInRadius.Add(ep.UserId);
                }
            }

            return usersInRadius;
        }

        public List<int> GetParticipants(int encounterId)
        {
            List<int> participants = new List<int>();
            var encounterProgresses = _repo.GetAll()
                .Where(ep => ep.EncounterId == encounterId && ep.Status == EncounterProgressStatus.Active)
                .ToList();
            foreach (var ep in encounterProgresses)
            {
                participants.Add(ep.UserId);
            }
            return participants;
        }
        public void FinishEncounterProgress(List<EncounterProgress> encounterProgresses, List<int> users)
        {
            foreach (var ep in encounterProgresses)
            {
                if (users.Contains(ep.UserId))
                {
                    ep.SetCompleted();
                    _repo.Update(ep);
                }
            }
        }
    }
}
