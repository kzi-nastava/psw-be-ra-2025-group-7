using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Tourist
{
    [Collection("Sequential")]
    public class TouristToursServiceTests : BaseToursIntegrationTest
    {
        public TouristToursServiceTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void GetPublishedTours_ReturnsData()
        {
            using var scope = Factory.Services.CreateScope();

            var service = scope.ServiceProvider.GetRequiredService<ITouristToursService>();
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            dbContext.Tours.Any(t => t.Status == Core.Domain.TourStatus.Published)
                     .ShouldBeTrue("Test baza nema nijednu published turu!");

            var result = service.GetPublishedTours();

            result.ShouldNotBeNull();
            result.Count.ShouldBeGreaterThan(0);   
        }
    }
}
