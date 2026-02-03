using AutoMapper;
using Explorer.Encounters.API.Dtos;
using Explorer.Encounters.API.Public;
using Explorer.Encounters.Core.Domain;
using Explorer.Encounters.Core.Domain.RepositoryInterfaces;
using Explorer.Stakeholders.API.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
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
            if (_repo.GetAll().Any(ep => ep.EncounterId == dto.EncounterId && ep.UserId == dto.UserId && ep.Status == EncounterProgressStatus.Active))
            {
                throw new InvalidOperationException("EncounterProgress already exists for this user and encounter.");
            }
            var encounter = _encounterRepo.Get(dto.EncounterId)
        ?? throw new KeyNotFoundException("Encounter not found.");

            EncounterProgress encounterProgress;

            if (encounter.Type == EncounterType.Location)
            {
                encounterProgress = new EncounterProgress(
                    dto.EncounterId,
                    dto.UserId,
                    dto.EnteredPhotoRadiusAt ?? DateTime.UtcNow
                );
            }
            else
            {
                var status = EncounterProgressStatus.Active;
                 encounterProgress = new EncounterProgress(dto.EncounterId, dto.UserId, status);
            }


           
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
                bool isInside = IsUserAtLocation(encounter.Id, ep.UserId);

                if (isInside)
                {
                    usersInRadius.Add((int)ep.UserId);
                }
            }

            return usersInRadius;
        }

        public bool IsUserAtLocation(long encounterId, long userId)
        {
            var encounter = _encounterRepo.Get(encounterId)
                ?? throw new KeyNotFoundException("Encounter not found.");
            var userLocationDto = _userProfileLocService.GetLocation((int)userId);
            if (userLocationDto.Latitude == null || userLocationDto.Longitude == null)
                return false;
            bool isWithinRadius = GeoDistanceCalculator.IsWithinRadius(
                encounter.Location.Latitude,
                encounter.Location.Longitude,
                encounter.Location.Radius ?? 0,
                userLocationDto.Latitude.Value,
                userLocationDto.Longitude.Value
            );
            return isWithinRadius;
        }

        public int GetActiveParticipants(int encounterId)
        {
            List<int> participants = new List<int>();
            var encounterProgresses = _repo.GetAll()
                .Where(ep => ep.EncounterId == encounterId && ep.Status == EncounterProgressStatus.Active)
                .ToList();
            return encounterProgresses.Count;
        }
        public void ActivateHiddenLocationForUser(long encounterId, long userId)
        {
            bool alreadyActive = _repo.GetAll().Any(p =>
                p.EncounterId == encounterId &&
                p.UserId == userId &&
                p.Status == EncounterProgressStatus.Active);

            if (alreadyActive)
                throw new InvalidOperationException("Encounter already activated.");

            var encounter = _encounterRepo.Get(encounterId)
                ?? throw new KeyNotFoundException("Encounter not found.");

            if (encounter.Type != EncounterType.Location)
                throw new InvalidOperationException("Not a hidden location encounter.");

            var hidden = encounter.HiddenLocationDetails
                ?? throw new InvalidOperationException("Hidden location not configured.");

            var userLocationDto = _userProfileLocService.GetLocation((int)userId);

            if (userLocationDto.Latitude == null || userLocationDto.Longitude == null)
                throw new InvalidOperationException("User location not available.");

            bool isWithinActivationRadius = GeoDistanceCalculator.IsWithinRadius(
                hidden.PhotoLocation.Latitude,
                hidden.PhotoLocation.Longitude,
                hidden.ActivationRadiusMeters,
                userLocationDto.Latitude.Value,
                userLocationDto.Longitude.Value
            );

            if (!isWithinActivationRadius)
                throw new InvalidOperationException("You are too far to activate this encounter.");

            var status = EncounterProgressStatus.Active;
            var encounterProgress = new EncounterProgress(encounterId, (int)userId, status);

            _repo.Create(encounterProgress);
        }
        public void OnUserLocationChanged(long userId)
        {
            var activeProgresses = _repo.GetAll()
                .Where(p => p.UserId == userId && p.Status == EncounterProgressStatus.Active)
                .ToList();

            foreach (var progress in activeProgresses)
            {
                var encounter = _encounterRepo.Get(progress.EncounterId);
                if (encounter == null) continue;

                if (encounter.Type != EncounterType.Location) continue;
                var hidden = encounter.HiddenLocationDetails;
                if (hidden == null) continue;

                var userLoc = _userProfileLocService.GetLocation((int)userId);
                if (userLoc == null || userLoc.Latitude == null || userLoc.Longitude == null) continue;


                bool inPhotoRadius = GeoDistanceCalculator.IsWithinRadius(
                    hidden.PhotoLocation.Latitude,
                    hidden.PhotoLocation.Longitude,
                    hidden.DistanceMeters,
                    userLoc.Latitude.Value,
                    userLoc.Longitude.Value
                );

                if (inPhotoRadius)
                {
                    progress.EnterPhotoRadius(DateTime.UtcNow);

                    if (progress.HasStayedLongEnough(hidden.SecondsToViewPhoto, DateTime.UtcNow))
                    {
                        progress.SetCompleted();
                    }
                }
                else
                {
                    progress.ExitPhotoRadius();
                }

                _repo.Update(progress);
            }
        }

        public EncounterProgressDto ActivateSocialEncounter(long encounterId, long userId)
        {
            bool alreadyActive = _repo.GetAll().Any(p =>
                p.EncounterId == encounterId &&
                p.UserId == userId &&
                p.Status == EncounterProgressStatus.Active);

            bool alreadyFinished = _repo.GetAll().Any(p =>
                p.EncounterId == encounterId &&
                p.UserId == userId &&
                p.Status == EncounterProgressStatus.Completed);

            if (alreadyActive)
                throw new InvalidOperationException("Encounter already activated.");

            if(alreadyFinished)
                throw new InvalidOperationException("Encounter already completed.");

            var encounter = _encounterRepo.Get(encounterId)
                ?? throw new KeyNotFoundException("Encounter not found.");

            var status = EncounterProgressStatus.Active;
            var encounterProgress = new EncounterProgress(encounterId, userId, status);
            _repo.Create(encounterProgress);
            return _mapper.Map<EncounterProgressDto>(encounterProgress);
        }

        public bool HasActiveSocialEncounter(long userId)
        {
            return _repo.GetAll()
                .Any(p =>
                {
                    var encounter = _encounterRepo.Get(p.EncounterId);
                    return encounter != null &&
                           encounter.Type == EncounterType.Social &&
                           p.UserId == userId &&
                           p.Status == EncounterProgressStatus.Active;
                });
        }

        public EncounterDto GetActiveSocial()
        {
            var activeSocial = _repo.GetAll()
                .FirstOrDefault(p =>
                {
                    var encounter = _encounterRepo.Get(p.EncounterId);
                    return encounter != null &&
                           encounter.Type == EncounterType.Social &&
                           p.Status == EncounterProgressStatus.Active;
                });

            if (activeSocial == null)
            {
                throw new InvalidOperationException("No active social encounter found.");
            }

            var encounterDetails = _encounterRepo.Get(activeSocial.EncounterId);

            if (encounterDetails == null)
            {
                throw new InvalidOperationException("Encounter not found for the active social progress.");
            }

            return _mapper.Map<EncounterDto>(encounterDetails);
        }


        public void FinishEncounterProgress(List<EncounterProgress> encounterProgresses, List<int> users)
        {
            foreach (var ep in encounterProgresses)
            {
                if (users.Contains((int)ep.UserId))
                {
                    ep.SetCompleted();
                    var encounter = _encounterRepo.Get(ep.EncounterId);
                    _userProfileLocService.AddXP((int)ep.UserId, encounter.Xp);
                    _repo.Update(ep);
                }
            }
        }
        public HiddenLocationProgressDto GetHiddenLocationProgress(long encounterId, long userId)
        {
            var progress = _repo.GetAll()
                .FirstOrDefault(p =>
                    p.EncounterId == encounterId &&
                    p.UserId == userId &&
                    p.Status == EncounterProgressStatus.Active);

            if (progress == null)
            {
                return new HiddenLocationProgressDto
                {
                    EncounterId = encounterId,
                    IsInPhotoRadius = false,
                    SecondsSpent = 0,
                    SecondsRequired = 0,
                    IsCompleted = false,
                    Message = "Encounter not activated."
                };
            }

            var encounter = _encounterRepo.Get(encounterId);
            var hidden = encounter.HiddenLocationDetails;

            if (hidden == null)
                throw new InvalidOperationException("Hidden location not configured.");

            int secondsSpent = 0;
            bool isInRadius = progress.EnteredPhotoRadiusAt != null;

            if (isInRadius)
            {
                secondsSpent = (int)(DateTime.UtcNow - progress.EnteredPhotoRadiusAt.Value).TotalSeconds;
                secondsSpent = Math.Min(secondsSpent, hidden.SecondsToViewPhoto);
            }

            return new HiddenLocationProgressDto
            {
                EncounterId = encounterId,
                IsInPhotoRadius = isInRadius,
                SecondsRequired = hidden.SecondsToViewPhoto,
                SecondsSpent = secondsSpent,
                IsCompleted = progress.Status == EncounterProgressStatus.Completed,
                Message = progress.Status == EncounterProgressStatus.Completed
                    ? "Encounter completed."
                    : isInRadius
                        ? $"Stay here: {secondsSpent} / {hidden.SecondsToViewPhoto} seconds"
                        : "You are too far."
            };
        }

        public EncounterProgressDto ActivateMiscEncounter(long encounterId, long userId)
        {
            bool alreadyActive = _repo.GetAll().Any(p =>
                p.EncounterId == encounterId &&
                p.UserId == userId &&
                p.Status == EncounterProgressStatus.Active);

            bool alreadyFinished = _repo.GetAll().Any(p =>
                p.EncounterId == encounterId &&
                p.UserId == userId &&
                p.Status == EncounterProgressStatus.Completed);

            if (alreadyActive)
                throw new InvalidOperationException("Encounter already activated.");

            if (alreadyFinished)
                throw new InvalidOperationException("Encounter already completed.");

            var encounter = _encounterRepo.Get(encounterId)
                ?? throw new KeyNotFoundException("Encounter not found.");

            var status = EncounterProgressStatus.Completed;
            var encounterProgress = new EncounterProgress(encounterId, userId, status);
            _userProfileLocService.AddXP((int)userId, encounter.Xp);
            _repo.Create(encounterProgress);
            return _mapper.Map<EncounterProgressDto>(encounterProgress);
        }

    }
}
