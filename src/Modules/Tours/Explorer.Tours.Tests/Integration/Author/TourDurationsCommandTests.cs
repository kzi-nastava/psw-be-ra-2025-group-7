using Explorer.API.Controllers.Author;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class TourDurationsCommandTests : BaseToursIntegrationTest
    {
        public TourDurationsCommandTests(ToursTestFactory factory) : base(factory) { }

        //[Fact]
        //public void Add_TourDuration_Succeeds()
        //{
        //    using var scope = Factory.Services.CreateScope();
        //    var controller = CreateController(scope);
        //    var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        //    var tourId = -6; // Draft tura
        //    var durationDto = new TourDurationDto { Type = 2, Minutes = 40 };

        //    var actionResult = controller.AddTourDuration(tourId, durationDto).Result;
        //    actionResult.ShouldBeOfType<OkObjectResult>();
        //    var result = (actionResult as OkObjectResult)?.Value as TourDto;

        //    result.ShouldNotBeNull();
        //    result.TourDurations.ShouldContain(d => d.Type == 2 && d.Minutes == 40);
        //}

        [Fact]
        public void Delete_TourDuration_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var tourId = -2;
            var typeToDelete = 1;
            var minutesToDelete = 50;

            // Pronađi tour sa svim duration-ima
            var tour = dbContext.Tours
                .Include(t => t.TourDurations) 
                .FirstOrDefault(t => t.Id == tourId);

            tour.ShouldNotBeNull();

            // Pronađi index duration-a koji želimo da obrišemo
            var index = 0;

            var actionResult = controller.RemoveTourDuration(tourId, index).Result;
            actionResult.ShouldBeOfType<OkObjectResult>();

            var result = (actionResult as OkObjectResult)?.Value as TourDto;
            result.ShouldNotBeNull();
            result.TourDurations.ShouldNotContain(d => (int)d.Type == typeToDelete && d.Minutes == minutesToDelete);
        }


        [Fact]
        public void Publish_Tour_Fails_WithoutDuration()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var tourId = -501; // Draft tura bez TourDuration

            var result = controller.Publish(tourId).Result;

            result.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.ShouldNotBeNull();
            badRequest.Value.ShouldBe("Cannot publish tour without at least one duration.");
        }

        [Fact]
        public void Publish_Tour_Succeeds_WithDuration()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var tourId = -500; // Draft tura koja već ima duration

            var actionResult = controller.Publish(tourId).Result;
            actionResult.ShouldBeOfType<OkObjectResult>();
            var result = (actionResult as OkObjectResult)?.Value as TourDto;

            result.ShouldNotBeNull();
            result.Status.ShouldBe(1); // Published
            result.PublishedAt.ShouldNotBeNull();
        }

        [Fact]
        public void Update_TourDuration_Succeeds()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var tourId = -2;
            var durationId = 0; // pretpostavljamo da postoji
            var durationDto = new TourDurationDto { Type = 2, Minutes = 60 };

            var actionResult = controller.UpdateTourDuration(tourId, durationId, durationDto).Result;
            actionResult.ShouldBeOfType<OkObjectResult>();
            var result = (actionResult as OkObjectResult)?.Value as TourDto;

            result.ShouldNotBeNull();
            result.TourDurations.ShouldContain(d => d.Type == 2 && d.Minutes == 60);
        }
        private static TourAuthoringController CreateController(IServiceScope scope)
        {
            return new TourAuthoringController(
                scope.ServiceProvider.GetRequiredService<ITourService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }
}
