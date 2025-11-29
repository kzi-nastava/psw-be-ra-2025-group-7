using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Explorer.BuildingBlocks.Core.Domain;


namespace Explorer.Stakeholders.Core.Domain
{
    public enum Difficulty
    {
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public class TourPreferences : Entity
    {
        public long TouristId { get; private set; }
        public Difficulty PreferredDifficulty { get; private set; }

        public int WalkingRating { get; private set; }
        public int BicycleRating { get; private set; }
        public int BoatRating { get; private set; }
        public int CarRating { get; private set; }
        public List<string> Tags { get; private set; }

        private TourPreferences() { }

        public TourPreferences(
            long touristId,
            Difficulty preferredDifficulty,
            int walkingRating,
            int bicycleRating,
            int carRating,
            int boatRating,
            List<string> tags)
        {
            TouristId = touristId;
            PreferredDifficulty = ValidateDifficulty(preferredDifficulty);

            WalkingRating = ValidateRating(walkingRating);
            BicycleRating = ValidateRating(bicycleRating);
            CarRating = ValidateRating(carRating);
            BoatRating = ValidateRating(boatRating);

            Tags = tags ?? new List<string>();
        }

        private static Difficulty ValidateDifficulty(Difficulty difficulty)
        {
            if (!Enum.IsDefined(typeof(Difficulty), difficulty))
                throw new ArgumentException("Invalid difficulty value.", nameof(difficulty));

            return difficulty;
        }

        private static int ValidateRating(int rating)
        {
            if (rating < 0 || rating > 3)
                throw new ArgumentOutOfRangeException("Score must be between 0 and 3.");
            else
                return rating;
        }

  

        public void Update(
            Difficulty preferredDifficulty,
            int walkingRating,
            int bicycleRating,
            int carRating,
            int boatRating,
            List<string> tags)
        {
            PreferredDifficulty = ValidateDifficulty(preferredDifficulty);

            WalkingRating = ValidateRating(walkingRating);
            BicycleRating = ValidateRating(bicycleRating);
            CarRating = ValidateRating(carRating);
            BoatRating = ValidateRating(boatRating);

            Tags = tags ?? new List<string>();
        }
    }

}
       