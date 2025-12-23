using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.Core.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Explorer.Encounters.Core.Domain.EncounterProgress;

namespace Explorer.Encounters.Core.UseCases
{
    public class EncounterProgressService
    {
        private readonly IEncounterProgressRepository _repo;
        private readonly IEncounterRepository _encounterRepo;
        private readonly IMapper _mapper;
        private readonly IUserProfileRepository _userProfileRepo;
        private List<int> users;

        public EncounterProgressService(IEncounterProgressRepository repo, IMapper mapper, IEncounterRepository encounterRepo, IUserProfileRepository userProfileRepo)
        {
            _repo = repo;
            _mapper = mapper;
            _encounterRepo = encounterRepo;
            _userProfileRepo = userProfileRepo;

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
            var users = new List<int>();

            foreach (var e in encounterProgresses)
            {
                var user = _userProfileRepo.GetByUserId(e.UserId);

                bool isInside = GeoDistanceCalculator.IsWithinRadius(
                    encounter.Location.Latitude,
                    encounter.Location.Longitude,
                    encounter.Location.Radius ?? 0,
                    user.CurrentLatitude ?? 0,
                    user.CurrentLongitude ?? 0
                );

                if (isInside)
                {
                    users.Add(e.UserId);
                }
            }

            return users;
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
