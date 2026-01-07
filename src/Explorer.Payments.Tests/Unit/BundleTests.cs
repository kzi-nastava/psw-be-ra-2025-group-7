using System;
using System.Collections.Generic;
using System.Linq;
using Explorer.Payments.Core.Domain;
using Xunit;

namespace Explorer.Payments.Tests.Unit
{
    public class BundleTests
    {
        [Fact]
        public void Constructor_ShouldCreateDraftBundle_WithDistinctItems()
        {
            // arrange
            var authorId = -11;
            var tourIds = new List<long> { -11, -8, -11 }; // duplikat namerno

            // act
            var bundle = new Bundle(authorId, "Prvi Bundle", 3500m, tourIds);

            // assert
            Assert.Equal(authorId, bundle.AuthorId);
            Assert.Equal("Prvi Bundle", bundle.Name);
            Assert.Equal(3500m, bundle.Price);

            // Draft je 0 (kao kod TourStatus)
            Assert.Equal(0, (int)bundle.Status);

            var actual = bundle.Items.Select(i => i.TourId).OrderBy(x => x).ToList();
            var expected = tourIds.Distinct().OrderBy(x => x).ToList();
            Assert.Equal(expected, actual);

            Assert.True(bundle.CreatedAt <= bundle.UpdatedAt);
        }

        [Fact]
        public void Update_ShouldReplaceItems_NotAppend()
        {
            // arrange
            var bundle = new Bundle(-11, "Prvi Bundle", 3500m, new List<long> { -11, -8 });

            // act
            bundle.Update("PRVI PAKET IZMENJEN", 4000m, new List<long> { -11 });

            // assert
            Assert.Equal("PRVI PAKET IZMENJEN", bundle.Name);
            Assert.Equal(4000m, bundle.Price);

            var tourIds = bundle.Items.Select(i => i.TourId).ToList();
            Assert.Single(tourIds);
            Assert.Contains(-11, tourIds);
            Assert.DoesNotContain(-8, tourIds);
        }

        [Fact]
        public void Update_ShouldThrow_WhenTourIdsEmpty()
        {
            // arrange
            var bundle = new Bundle(-11, "Prvi Bundle", 3500m, new List<long> { -11 });

            // act + assert
            Assert.ThrowsAny<Exception>(() =>
                bundle.Update("Izmena", 100m, new List<long>())
            );
        }
    }
}
