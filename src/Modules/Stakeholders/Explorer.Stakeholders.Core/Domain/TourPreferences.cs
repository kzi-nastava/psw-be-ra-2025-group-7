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
        VeryEasy = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public enum TransportMode
    {
        Walking = 0,
        Bicycle = 1,
        Car = 2,
        Boat = 3
    }

    public class TourPreferences : Entity
    {
        public long TouristId { get; private set; }
        public Difficulty PreferredDifficulty { get; private set; }

        // Mapa sada: TransportMode -> Difficulty
        public Dictionary<TransportMode, Difficulty> TransportDifficulties { get; private set; }

        public List<string> Tags { get; private set; }

        private TourPreferences() { }

        public TourPreferences(
            long touristId,
            Difficulty preferredDifficulty,
            Difficulty walkingDifficulty,
            Difficulty bicycleDifficulty,
            Difficulty carDifficulty,
            Difficulty boatDifficulty,
            List<string> tags)
        {
            TouristId = touristId;
            PreferredDifficulty = ValidateDifficulty(preferredDifficulty);

            TransportDifficulties = new Dictionary<TransportMode, Difficulty>
            {
                { TransportMode.Walking, ValidateDifficulty(walkingDifficulty) },
                { TransportMode.Bicycle, ValidateDifficulty(bicycleDifficulty) },
                { TransportMode.Car, ValidateDifficulty(carDifficulty) },
                { TransportMode.Boat, ValidateDifficulty(boatDifficulty) }
            };

            Tags = tags ?? new List<string>();
        }

        private static Difficulty ValidateDifficulty(Difficulty difficulty)
        {
            if (!Enum.IsDefined(typeof(Difficulty), difficulty))
                throw new ArgumentException("Invalid difficulty value.", nameof(difficulty));

            return difficulty;
        }

        public void SetDifficulty(TransportMode mode, Difficulty difficulty)
        {
            TransportDifficulties[mode] = ValidateDifficulty(difficulty);
        }

        public Difficulty GetDifficulty(TransportMode mode)
        {
            return TransportDifficulties.TryGetValue(mode, out var diff)
                ? diff
                : Difficulty.Easy; // default
        }

        public void Update(
            Difficulty preferredDifficulty,
            Difficulty walkingDifficulty,
            Difficulty bicycleDifficulty,
            Difficulty carDifficulty,
            Difficulty boatDifficulty,
            List<string> tags)
        {
            PreferredDifficulty = ValidateDifficulty(preferredDifficulty);

            TransportDifficulties[TransportMode.Walking] = ValidateDifficulty(walkingDifficulty);
            TransportDifficulties[TransportMode.Bicycle] = ValidateDifficulty(bicycleDifficulty);
            TransportDifficulties[TransportMode.Car] = ValidateDifficulty(carDifficulty);
            TransportDifficulties[TransportMode.Boat] = ValidateDifficulty(boatDifficulty);

            Tags = tags ?? new List<string>();
        }
    }

}
       