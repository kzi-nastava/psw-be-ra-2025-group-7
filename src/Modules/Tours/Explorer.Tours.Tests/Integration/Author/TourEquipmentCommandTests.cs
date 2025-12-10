using Explorer.API.Controllers.Author;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Administration;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Xunit;
using System.Threading.Tasks;

namespace Explorer.Tours.Tests.Integration.Author
{
    [Collection("Sequential")]
    public class TourEquipmentCommandTests : BaseToursIntegrationTest
    {
        public TourEquipmentCommandTests(ToursTestFactory factory) : base(factory) { }

        /// <summary>
        /// Pozitivan scenario: Autor može dodati opremu turi koja nije arhivirana (Draft).
        /// </summary>
        [Fact]
        public void AddEquipment_ToDraftTour_Succeeds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            // Kreiramo novu Draft turu za ovaj test
            var newTour = service.Create(new TourDto
            {
                AuthorId = -1,
                Name = "Draft Tour for Equipment Test",
                Description = "This tour is for testing equipment addition to draft tour",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Status = 0,
                Price = 0
            });

            var equipmentId = -1L; // Oprema "Voda"

            // Act
            ActionResult<TourDto> actionResultWrapper = controller.AddEquipment(newTour.Id, equipmentId);
            IActionResult actionResult = actionResultWrapper.Result;

            // Assert
            actionResult.ShouldBeOfType<OkObjectResult>();
            var result = (actionResult as OkObjectResult)?.Value as TourDto;

            result.ShouldNotBeNull();
            result.RequiredEquipment.ShouldNotBeNull();
            result.RequiredEquipment.ShouldContain(eq => eq.Id == equipmentId);
        }

        /// <summary>
        /// Pozitivan scenario: Autor može dodati opremu turi koja je Published.
        /// </summary>
        [Fact]
        public void AddEquipment_ToPublishedTour_Succeeds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

            var tourId = -3; // Published tura
            var equipmentId = -2L; // Štapovi za šetanje

            // Proveri da tura nije arhivirana
            var tour = dbContext.Tours.Find((long)tourId);
            tour.ShouldNotBeNull();
            tour.Status.ShouldNotBe(TourStatus.Archived);

            // Act
            ActionResult<TourDto> actionResultWrapper = controller.AddEquipment(tourId, equipmentId);
            IActionResult actionResult = actionResultWrapper.Result;

            // Assert
            actionResult.ShouldBeOfType<OkObjectResult>();
            var result = (actionResult as OkObjectResult)?.Value as TourDto;

            result.ShouldNotBeNull();
            result.RequiredEquipment.ShouldNotBeNull();
            result.RequiredEquipment.ShouldContain(eq => eq.Id == equipmentId);
        }

        /// <summary>
        /// Negativan scenario: Dodavanje opreme arhiviranoj turi treba da baci grešku.
        /// </summary>
        [Fact]
        public void AddEquipment_ToArchivedTour_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            // Kreiramo potpuno novu turu za ovaj test
            var newTour = service.Create(new TourDto
            {
                AuthorId = -1,
                Name = "Tour for Archive Equipment Test",
                Description = "This tour is specifically for testing archived equipment",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Status = 0,
                Price = 0
            });

            // Dodajemo KeyPoints da bi mogli da publish-ujemo
            service.AddKeyPoint(newTour.Id, -1, new KeyPointDto
            {
                Latitude = 44.7866,
                Longitude = 20.4489,
                Name = "KP1",
                Description = "First",
                ImageUrl = null,
                Secret = "Secret1"
            });
            service.AddKeyPoint(newTour.Id, -1, new KeyPointDto
            {
                Latitude = 44.7900,
                Longitude = 20.4500,
                Name = "KP2",
                Description = "Second",
                ImageUrl = null,
                Secret = "Secret2"
            });

            // Dodajemo TourDuration (bez eksplicitnog ID-a da izbegnemo konflikt)
            service.AddTourDuration(newTour.Id, -1, new TourDurationDto
            {
                Type = 1,
                Minutes = 60
            });

            // Publish-ujemo turu
            service.Publish(newTour.Id, -1);

            // Arhiviramo turu
            service.Archive(newTour.Id, -1);

            var equipmentId = -1L;

            // Act
            var result = controller.AddEquipment(newTour.Id, equipmentId).Result;

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.ShouldNotBeNull();
            badRequest.Value.ShouldBe("You cannot modify equipment for archived tours. Please reactivate the tour first.");
        }

        /// <summary>
        /// Negativan scenario: Dodavanje iste opreme dva puta treba da baci grešku.
        /// </summary>
        [Fact]
        public void AddEquipment_Duplicate_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            // Kreiramo novu Draft turu za ovaj test
            var newTour = service.Create(new TourDto
            {
                AuthorId = -1,
                Name = "Draft Tour for Duplicate Equipment Test",
                Description = "This tour is for testing duplicate equipment",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Status = 0,
                Price = 0
            });

            var equipmentId = -1L;

            // Prvo dodaj opremu
            controller.AddEquipment(newTour.Id, equipmentId);

            // Act - pokušaj ponovo da dodaš istu opremu
            var result = controller.AddEquipment(newTour.Id, equipmentId).Result;

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.ShouldNotBeNull();
            badRequest.Value.ShouldBe("Equipment is already added to this tour.");
        }

        /// <summary>
        /// Negativan scenario: Drugi autor ne sme da dodaje opremu tuđoj turi.
        /// </summary>
        [Fact]
        public void AddEquipment_AsDifferentAuthor_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            var tourId = -1L; // Tura autora -1
            var wrongAuthorId = -2L; // Drugi autor
            var equipmentId = -1L;

            // Act & Assert
            Should.Throw<ForbiddenException>(() =>
            {
                service.AddEquipment(tourId, wrongAuthorId, equipmentId);
            });
        }

        /// <summary>
        /// Negativan scenario: Dodavanje nepostojeće opreme treba da baci grešku.
        /// </summary>
        [Fact]
        public void AddEquipment_NonExistentEquipment_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var tourId = -1L;
            var nonExistentEquipmentId = -9999L;

            // Act & Assert
            Should.Throw<NotFoundException>(() =>
            {
                controller.AddEquipment(tourId, nonExistentEquipmentId);
            });
        }

        /// <summary>
        /// Pozitivan scenario: Autor može ukloniti opremu sa ture koja nije arhivirana.
        /// </summary>
        [Fact]
        public void RemoveEquipment_FromDraftTour_Succeeds()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            // Kreiramo novu Draft turu za ovaj test
            var newTour = service.Create(new TourDto
            {
                AuthorId = -1,
                Name = "Draft Tour for Remove Equipment Test",
                Description = "This tour is for testing equipment removal",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Status = 0,
                Price = 0
            });

            var equipmentId = -2L;

            // Prvo dodaj opremu
            controller.AddEquipment(newTour.Id, equipmentId);

            // Act - ukloni opremu
            var actionResult = controller.RemoveEquipment(newTour.Id, equipmentId).Result;

            // Assert
            actionResult.ShouldBeOfType<OkObjectResult>();
            var result = (actionResult as OkObjectResult)?.Value as TourDto;

            result.ShouldNotBeNull();
            result.RequiredEquipment.ShouldNotBeNull();
            result.RequiredEquipment.ShouldNotContain(eq => eq.Id == equipmentId);
        }

        /// <summary>
        /// Negativan scenario: Uklanjanje opreme sa arhivirane ture treba da baci grešku.
        /// </summary>
        [Fact]
        public void RemoveEquipment_FromArchivedTour_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            // Kreiramo potpuno novu turu za ovaj test
            var newTour = service.Create(new TourDto
            {
                AuthorId = -1,
                Name = "Tour for Remove Archived Equipment Test",
                Description = "This tour is specifically for testing removal from archived tour",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Status = 0,
                Price = 0
            });

            // Dodajemo KeyPoints
            service.AddKeyPoint(newTour.Id, -1, new KeyPointDto
            {
                Latitude = 44.7866,
                Longitude = 20.4489,
                Name = "KP1",
                Description = "First",
                ImageUrl = null,
                Secret = "Secret1"
            });
            service.AddKeyPoint(newTour.Id, -1, new KeyPointDto
            {
                Latitude = 44.7900,
                Longitude = 20.4500,
                Name = "KP2",
                Description = "Second",
                ImageUrl = null,
                Secret = "Secret2"
            });

            // Dodajemo TourDuration (bez eksplicitnog ID-a)
            service.AddTourDuration(newTour.Id, -1, new TourDurationDto
            {
                Type = 1,
                Minutes = 60
            });

            var equipmentId = -1L;

            // Publish-ujemo turu
            service.Publish(newTour.Id, -1);

            // Dodaj opremu pre arhiviranja
            service.AddEquipment(newTour.Id, -1, equipmentId);

            // Arhiviraj turu
            service.Archive(newTour.Id, -1);

            // Act - pokušaj da ukloniš opremu sa arhivirane ture
            var result = controller.RemoveEquipment(newTour.Id, equipmentId).Result;

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.ShouldNotBeNull();
            badRequest.Value.ShouldBe("You cannot modify equipment for archived tours. Please reactivate the tour first.");
        }

        /// <summary>
        /// Negativan scenario: Uklanjanje opreme koja nije na turi treba da baci grešku.
        /// </summary>
        [Fact]
        public void RemoveEquipment_NotOnTour_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            var tourId = -2L; // Draft tura bez opreme
            var equipmentId = -3L;

            // Act
            var result = controller.RemoveEquipment(tourId, equipmentId).Result;

            // Assert
            result.ShouldBeOfType<BadRequestObjectResult>();
            var badRequest = result as BadRequestObjectResult;
            badRequest.ShouldNotBeNull();
            badRequest.Value.ShouldBe("Equipment is not part of this tour.");
        }

        /// <summary>
        /// Negativan scenario: Drugi autor ne sme da uklanja opremu sa tuđe ture.
        /// </summary>
        [Fact]
        public void RemoveEquipment_AsDifferentAuthor_Fails()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            var tourId = -1L; // Tura autora -1
            var wrongAuthorId = -2L;
            var equipmentId = -1L;

            // Act & Assert
            Should.Throw<ForbiddenException>(() =>
            {
                service.RemoveEquipment(tourId, wrongAuthorId, equipmentId);
            });
        }

        /// <summary>
        /// Pozitivan scenario: Tura sa opremom vraća kompletan RequiredEquipment spisak.
        /// </summary>
        [Fact]
        public void GetTour_ReturnsRequiredEquipment()
        {
            // Arrange
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var service = scope.ServiceProvider.GetRequiredService<ITourService>();

            // Kreiramo novu Draft turu za ovaj test
            var newTour = service.Create(new TourDto
            {
                AuthorId = -1,
                Name = "Draft Tour for GetTour Equipment Test",
                Description = "This tour is for testing equipment retrieval",
                Difficulty = 1,
                Tags = new List<string> { "test" },
                Status = 0,
                Price = 0
            });

            var equipmentId1 = -1L;
            var equipmentId2 = -2L;

            // Dodaj opremu
            controller.AddEquipment(newTour.Id, equipmentId1);
            controller.AddEquipment(newTour.Id, equipmentId2);

            // Act
            var actionResult = controller.GetAll(1, 20).Result; // Povećan pageSize
            actionResult.ShouldBeOfType<OkObjectResult>();
            var pagedResult = (actionResult as OkObjectResult)?.Value as BuildingBlocks.Core.UseCases.PagedResult<TourDto>;

            // Assert
            pagedResult.ShouldNotBeNull();
            var tour = pagedResult.Results.FirstOrDefault(t => t.Id == newTour.Id);
            tour.ShouldNotBeNull();
            tour.RequiredEquipment.ShouldNotBeNull();
            tour.RequiredEquipment.Count.ShouldBe(2);
            tour.RequiredEquipment.ShouldContain(eq => eq.Id == equipmentId1);
            tour.RequiredEquipment.ShouldContain(eq => eq.Id == equipmentId2);
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