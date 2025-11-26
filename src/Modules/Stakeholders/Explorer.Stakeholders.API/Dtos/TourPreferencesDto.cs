using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Stakeholders.API.Dtos
{
    // enumi razdvojeni od modela zbog nezavisti
    public enum DifficultyDto
    {
        VeryEasy = 0,
        Easy = 1,
        Medium = 2,
        Hard = 3
    }

    public enum TransportModeDto
    {
        Walking = 0,
        Bicycle = 1,
        Car = 2,
        Boat = 3
    }

    //Lista umesto mape radi lakseg rada na frontu
    public class TransportDifficultyDto
    {
        public TransportModeDto TransportMode { get; set; }
        public DifficultyDto Difficulty { get; set; }
    }

    // Glavni DTO
    public class TourPreferencesDto
    {
        public long Id { get; set; }                 
        public long TouristId { get; set; }

        public DifficultyDto PreferredDifficulty { get; set; }

        public List<TransportDifficultyDto> TransportDifficulties { get; set; }

        public List<string> Tags { get; set; }

        public TourPreferencesDto()
        {
            TransportDifficulties = new List<TransportDifficultyDto>();
            Tags = new List<string>();
        }
    }
}

﻿