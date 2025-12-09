using System;
using System.Collections.Generic;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;

namespace Explorer.Stakeholders.Tests.Integration
{
    [Collection("Sequential")]
    public class TourPreferencesCommandTests : BaseStakeholdersIntegrationTest, IDisposable
    {
        private readonly ITourPreferencesService _service;
        private readonly StakeholdersContext _dbContext;
        private readonly IServiceScope _scope;

        public TourPreferencesCommandTests(StakeholdersTestFactory factory) : base(factory)
        {
            // Kreiramo scoped servis
            _scope = Factory.Services.CreateScope();
            _dbContext = _scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
            _service = _scope.ServiceProvider.GetRequiredService<ITourPreferencesService>();

            // Resetujemo tabelu pre svakog testa
            _dbContext.TourPreferences.RemoveRange(_dbContext.TourPreferences);
            _dbContext.SaveChanges();

            // Ubacujemo test podatke direktno u kontekst (uskladjeno sa novim SQL-om)
            _dbContext.TourPreferences.AddRange(new[]
            {
                new Core.Domain.TourPreferences(-21, Core.Domain.Difficulty.Hard, 3, 3, 0, 0, new List<string>{"hiking","mountain"}),
                new Core.Domain.TourPreferences(-22, Core.Domain.Difficulty.Medium, 0, 1, 3, 2, new List<string>{"relax","river"}),
                new Core.Domain.TourPreferences(-23, Core.Domain.Difficulty.Hard, 2, 2, 2, 3, new List<string>{"food","culture"})
            });
            _dbContext.SaveChanges();
        }

        public void Dispose()
        {
            _scope.Dispose();
        }

        [Fact]
        public void Create_preferences_success()
        {
            var dto = new TourPreferencesDto
            {
                TouristId = -50,
                PreferredDifficulty = 2, // Hard
                WalkingRating = 3,
                BicycleRating = 2,
                CarRating = 1,
                BoatRating = 0,
                Tags = new List<string> { "nature", "hill" }
            };

            var result = _service.Create(dto);

            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(-50);
            result.PreferredDifficulty.ShouldBe(2);
            result.WalkingRating.ShouldBe(3);
            result.Tags.Count.ShouldBe(2);
        }

        [Fact]
        public void Update_preferences_success()
        {
            var existing = _service.GetByTouristId(-21);
            existing.ShouldNotBeNull();

            existing.PreferredDifficulty = 1; // Medium
            existing.BoatRating = 3;
            existing.Tags = new List<string> { "updated", "tag" };

            var updated = _service.Update(existing);

            updated.ShouldNotBeNull();
            updated.PreferredDifficulty.ShouldBe(1);
            updated.BoatRating.ShouldBe(3);
            updated.Tags.Count.ShouldBe(2);
        }

        [Fact]
        public void Update_nonexistent_preferences_fails()
        {
            var dto = new TourPreferencesDto
            {
                Id = 99999,
                TouristId = 99999,
                PreferredDifficulty = 1,
                WalkingRating = 1,
                BicycleRating = 1,
                CarRating = 1,
                BoatRating = 1,
                Tags = new List<string>()
            };

            Should.Throw<InvalidOperationException>(() => _service.Update(dto));
        }

        [Fact]
        public void Get_preferences_by_tourist_id_success()
        {
            var result = _service.GetByTouristId(-22);

            result.ShouldNotBeNull();
            result.TouristId.ShouldBe(-22);
            result.PreferredDifficulty.ShouldBe(1); // Medium
        }

        [Fact]
        public void Delete_preferences_success()
        {
            var existing = _service.GetByTouristId(-23);
            existing.ShouldNotBeNull();

            _service.Delete(existing.Id);

            var result = _service.GetByTouristId(-23);
            result.ShouldBeNull();
        }

        [Fact]
        public void Create_preferences_invalid_rating_fails()
        {
            var dto = new TourPreferencesDto
            {
                TouristId = -51,
                PreferredDifficulty = 1,
                WalkingRating = 5, // invalid
                BicycleRating = 0,
                CarRating = 1,
                BoatRating = 2
            };

            Should.Throw<ArgumentOutOfRangeException>(() => _service.Create(dto));
        }
    }
}