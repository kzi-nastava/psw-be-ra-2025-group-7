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

        public TourPreferencesDto Create(TourPreferencesDto tourPreferencesDto)
        {
            var entity = new TourPreferences(
                tourPreferencesDto.TouristId,
                (Difficulty)ValidateDifficulty(tourPreferencesDto.PreferredDifficulty),
                ValidateRating(tourPreferencesDto.WalkingRating),
                ValidateRating(tourPreferencesDto.BicycleRating),
                ValidateRating(tourPreferencesDto.CarRating),
                ValidateRating(tourPreferencesDto.BoatRating),
                tourPreferencesDto.Tags ?? new List<string>()
                );
            var created = _repository.Create(entity);
            return _mapper.Map<TourPreferencesDto>(created);
        }

        public TourPreferencesDto Update(TourPreferencesDto tourPreferencesDto)
        {
            var entity = _repository.GetByTouristId(tourPreferencesDto.TouristId);
            if(entity == null || entity.Id != tourPreferencesDto.Id)
            {
                throw new InvalidOperationException("Preferences not found.");
            }
            entity.Update(
                (Difficulty)ValidateDifficulty(tourPreferencesDto.PreferredDifficulty),
                ValidateRating(tourPreferencesDto.WalkingRating),
                ValidateRating(tourPreferencesDto.BicycleRating),
                ValidateRating(tourPreferencesDto.CarRating),
                ValidateRating(tourPreferencesDto.BoatRating),
                tourPreferencesDto.Tags ?? new List<string>()
                );
            var updated = _repository.Update(entity);
            return _mapper.Map<TourPreferencesDto>(updated);
        }

        public void Delete(long touristId)
        {
            _repository.Delete(touristId);
        }


        private int ValidateDifficulty(int difficulty)
        {
            if (difficulty < 0 || difficulty > 2)
                throw new ArgumentOutOfRangeException("Difficulty must be between 0 and 3.");
            else
                return difficulty;
        }

        private int ValidateRating(int rating)
        {
            if (rating < 0 || rating > 3)
                throw new ArgumentOutOfRangeException("Score must be between 0 and 3.");
            else
                return rating;
        }


    }
}






