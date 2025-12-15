using System;
using System.Collections.Generic;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;
using Shouldly;
using Xunit;

namespace Explorer.Tours.Tests
{
    public class TourLengthTests
    {
        [Fact]
        public void Length_is_zero_when_less_than_two_keypoints()
        {
            // Arrange
            var tour = new Tour(
                authorId: -1,
                name: "Test tour",
                description: "Desc",
                difficulty: TourDifficulty.Medium,
                tags: new List<string> { "test" });

            // Act + Assert: bez keypoint-ova
            tour.LengthInKm.ShouldBe(0);

            // Dodajemo samo jedan keypoint
            tour.AddKeyPoint(new KeyPoint(
                latitude: 45.0,
                longitude: 19.0,
                name: "A",
                description: "A",
                secret: "secret-1",
                imageUrl: null));

            // I dalje 0
            tour.LengthInKm.ShouldBe(0);
        }

        [Fact]
        public void Length_is_calculated_from_two_keypoints()
        {
            var tour = new Tour(
                authorId: -1,
                name: "Test tour",
                description: "Desc",
                difficulty: TourDifficulty.Medium,
                tags: new List<string> { "test" });

            // Dva point-a ~111 km udaljena (0,0) -> (0,1)
            tour.AddKeyPoint(new KeyPoint(
                latitude: 0.0,
                longitude: 0.0,
                name: "A",
                description: "A",
                secret: "s1",
                imageUrl: null));

            tour.AddKeyPoint(new KeyPoint(
                latitude: 0.0,
                longitude: 1.0,
                name: "B",
                description: "B",
                secret: "s2",
                imageUrl: null));

            tour.LengthInKm.ShouldBeInRange(110, 112);
        }

        [Fact]
        public void Cannot_modify_keypoints_when_not_in_draft()
        {
            var tour = new Tour(
                authorId: -1,
                name: "Test tour",
                description: "Desc",
                difficulty: TourDifficulty.Medium,
                tags: new List<string> { "test" });

            tour.AddKeyPoint(new KeyPoint(
                latitude: 0.0,
                longitude: 0.0,
                name: "A",
                description: "A",
                secret: "s1",
                imageUrl: null));

            tour.AddKeyPoint(new KeyPoint(
                latitude: 0.0,
                longitude: 1.0,
                name: "B",
                description: "B",
                secret: "s2",
                imageUrl: null));

            // barem jedna duration da bi Publish prosao
            tour.AddDuration(new TourDuration(TravelType.Walk, 60));

            // Publish -> vise nije Draft
            tour.Publish();

            Should.Throw<InvalidOperationException>(() =>
                tour.AddKeyPoint(new KeyPoint(
                    latitude: 0.0,
                    longitude: 2.0,
                    name: "C",
                    description: "C",
                    secret: "s3",
                    imageUrl: null)));
        }
    }
}
