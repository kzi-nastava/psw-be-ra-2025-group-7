using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Core.Domain.RepositoryInterfaces;

namespace Explorer.Stakeholders.Core.UseCases
{
    public class TourPreferencesService : ITourPreferencesService
    {
        private readonly ITourPreferencesRepository _repository;
        private readonly IMapper _mapper;

        public TourPreferencesService(
            ITourPreferencesRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public TourPreferencesDto GetByTouristId(long touristId)
        {
            var entity = _repository.GetByTouristId(touristId);
            if (entity == null) return null;
            return _mapper.Map<TourPreferencesDto>(entity);
        }

        public TourPreferencesDto Create(TourPreferencesDto dto)
        {
            // Validacija preferred difficulty
            ValidateDifficulty(dto.PreferredDifficulty);

            // Validacija transport difficulty
            foreach (var t in dto.TransportDifficulties ?? new List<TransportDifficultyDto>())
            {
                ValidateDifficulty(t.Difficulty);
            }

            // Kreiranje domen entity-ja
            var transportMap = dto.TransportDifficulties?.ToDictionary(
                x => (TransportMode)x.TransportMode,
                x => (Difficulty)x.Difficulty
            ) ?? new Dictionary<TransportMode, Difficulty>();

            var entity = new TourPreferences(
                dto.TouristId,
                (Difficulty)dto.PreferredDifficulty,
                transportMap.ContainsKey(TransportMode.Walking) ? transportMap[TransportMode.Walking] : Difficulty.VeryEasy,
                transportMap.ContainsKey(TransportMode.Bicycle) ? transportMap[TransportMode.Bicycle] : Difficulty.VeryEasy,
                transportMap.ContainsKey(TransportMode.Car) ? transportMap[TransportMode.Car] : Difficulty.VeryEasy,
                transportMap.ContainsKey(TransportMode.Boat) ? transportMap[TransportMode.Boat] : Difficulty.VeryEasy,
                dto.Tags ?? new List<string>()
            );

            var created = _repository.Create(entity);
            return _mapper.Map<TourPreferencesDto>(created);
        }

        public TourPreferencesDto Update(TourPreferencesDto dto)
        {
            ValidateDifficulty(dto.PreferredDifficulty);

            foreach (var t in dto.TransportDifficulties ?? new List<TransportDifficultyDto>())
            {
                ValidateDifficulty(t.Difficulty);
            }

            var entity = _repository.GetByTouristId(dto.TouristId);
            if (entity == null)
                throw new InvalidOperationException("Preferences not found.");

            // Pretvaranje liste u mapu
            var transportMap = dto.TransportDifficulties?.ToDictionary(
                x => (TransportMode)x.TransportMode,
                x => (Difficulty)x.Difficulty
            ) ?? new Dictionary<TransportMode, Difficulty>();

            // Update domenskog entity-ja
            entity.Update(
                (Difficulty)dto.PreferredDifficulty,
                transportMap.ContainsKey(TransportMode.Walking) ? transportMap[TransportMode.Walking] : Difficulty.VeryEasy,
                transportMap.ContainsKey(TransportMode.Bicycle) ? transportMap[TransportMode.Bicycle] : Difficulty.VeryEasy,
                transportMap.ContainsKey(TransportMode.Car) ? transportMap[TransportMode.Car] : Difficulty.VeryEasy,
                transportMap.ContainsKey(TransportMode.Boat) ? transportMap[TransportMode.Boat] : Difficulty.VeryEasy,
                dto.Tags ?? new List<string>()
            );

            var updated = _repository.Update(entity);
            return _mapper.Map<TourPreferencesDto>(updated);
        }

        public void Delete(long touristId)
        {
            _repository.Delete(touristId);
        }

        private void ValidateDifficulty(DifficultyDto difficulty)
        {
            if (!Enum.IsDefined(typeof(Difficulty), (Difficulty)difficulty))
            {
                throw new ArgumentException($"Invalid difficulty value: {difficulty}");
            }
        }
    }
}








