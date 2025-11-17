using Explorer.API.Controllers.Administrator.Administration;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class FacilityCommandTests : BaseToursIntegrationTest
    {
        public FacilityCommandTests(ToursTestFactory factory) : base(factory) { }

        [Fact]
        public void Creates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var newEntity = new FacilityDto
            {
                Id = -10,
                Name = "Test Facility 10",
                Latitude = 45.2671,
                Longitude = 19.8335,
                Category = "Parking"
            };

            var result = ((ObjectResult)controller.Create(newEntity).Result)?.Value as FacilityDto;

            result.ShouldNotBeNull();
            result.Id.ShouldNotBe(0);
            result.Name.ShouldBe(newEntity.Name);

            var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Name == newEntity.Name);
            storedEntity.ShouldNotBeNull();
            storedEntity.Id.ShouldBe(result.Id);
        }

        [Fact]
        public void Updates()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var updatedEntity = new FacilityDto
            {
                Id = -1, 
                Name = "Updated Facility",
                Latitude = 45.2671,
                Longitude = 19.8335,
                Category = "Parking"
            };

            var result = ((ObjectResult)controller.Update(updatedEntity).Result)?.Value as FacilityDto;

            result.ShouldNotBeNull();
            result.Id.ShouldBe(-1);
            result.Name.ShouldBe(updatedEntity.Name);

            var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == -1);
            storedEntity.Name.ShouldBe(updatedEntity.Name);
        }

        [Fact]
        public void Deletes()
        {
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var result = (OkResult)controller.Delete(-1);

            result.ShouldNotBeNull();
            result.StatusCode.ShouldBe(200);

            var storedEntity = dbContext.Facility.FirstOrDefault(i => i.Id == -1);
            storedEntity.ShouldBeNull();
        }

        private static FacilityController CreateController(IServiceScope scope)
        {
            return new FacilityController(scope.ServiceProvider.GetRequiredService<IFacilityService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }

}
