using Explorer.API.Controllers.Tourist;
using Explorer.BuildingBlocks.Core.Exceptions;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;
using System.Linq;

namespace Explorer.Stakeholders.Tests.Integration.Clubs;

[Collection("Sequential")]
public class ClubCommandTest : BaseStakeholdersIntegrationTest
{
    public ClubCommandTest(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void Creates()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var newEntity = new ClubDto
        {
            Name = "Test klub",
            Description = "Opis test kluba",
            CreatedBy = 1,
            ImageUrls = new List<string> { "image1.jpg" }
        };

        // Act
        var actionResult = controller.Create(newEntity);
        var result = (actionResult.Result as OkObjectResult)?.Value as ClubDto;

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldNotBe(0);
        result.Name.ShouldBe(newEntity.Name);
        result.Description.ShouldBe(newEntity.Description);
        result.ImageUrls.ShouldBeEquivalentTo(newEntity.ImageUrls);
        result.CreatedBy.ShouldBe(newEntity.CreatedBy);
        result.CreatedAt.ShouldNotBe(default);
        result.UpdatedAt.ShouldNotBe(default);
        result.Status.ShouldBe(ClubStatusDto.Active);


        // Assert - Database
        var storedEntity = dbContext.Clubs.FirstOrDefault(c => c.Id == result.Id);
        storedEntity.ShouldNotBeNull();
        storedEntity.Id.ShouldBe(result.Id);
        storedEntity.Name.ShouldBe(newEntity.Name);
        storedEntity.Description.ShouldBe(newEntity.Description);
        storedEntity.CreatedBy.ShouldBe(newEntity.CreatedBy);
        storedEntity.CreatedAt.ShouldNotBe(default);
        storedEntity.UpdatedAt.ShouldNotBe(default);
        storedEntity.Status.ShouldBe(ClubStatus.Active);
    }

    [Fact]
    public void Create_fails_invalid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new ClubDto
        {
            Description = "Test"
        };

        // Act & Assert
        Should.Throw<ArgumentException>(() => controller.Create(updatedEntity));
    }

    [Fact]
    public void Updates()
    {
        long idToUpdate;

        using (var createdScope = Factory.Services.CreateScope())
        {
            var createController = CreateController(createdScope);

            var created = createController.Create(new ClubDto
            {
                Name = "Pocetno ime",
                Description = "Pocetni opis",
                CreatedBy = 1,
                ImageUrls = new List<string> { "initial.jpg" }
            });

            var createdResult = (created.Result as OkObjectResult)?.Value as ClubDto;
            createdResult.ShouldNotBeNull();
            createdResult.Id.ShouldNotBe(0);
            idToUpdate = createdResult.Id;
        }

        using (var updatedScope = Factory.Services.CreateScope())
        {
            var controller = CreateController(updatedScope);
            var dbContext = updatedScope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            var updatedEntity = new ClubDto
            {
                Id = idToUpdate,
                Name = "Ažurirano ime",
                Description = "Ažurirani opis",
                CreatedBy = 1,
                ImageUrls = new List<string> { "updated.jpg" }
            };

            // Act
            var actionResult = controller.Update(idToUpdate, updatedEntity);
            var result = (actionResult.Result as OkObjectResult)?.Value as ClubDto;

            // Assert - Response
            result.ShouldNotBeNull();
            result.Id.ShouldBe(idToUpdate);
            result.Name.ShouldBe(updatedEntity.Name);
            result.Description.ShouldBe(updatedEntity.Description);
            result.ImageUrls.ShouldBe(updatedEntity.ImageUrls);
            result.CreatedBy.ShouldBe(updatedEntity.CreatedBy);
            result.CreatedAt.ShouldNotBe(default);
            result.UpdatedAt.ShouldNotBe(default);
            result.UpdatedAt.ShouldBeGreaterThanOrEqualTo(result.CreatedAt);

            // Assert - Database
            var storedEntity = dbContext.Clubs.FirstOrDefault(c => c.Id == idToUpdate);
            storedEntity.ShouldNotBeNull();
            storedEntity.Name.ShouldBe(updatedEntity.Name);
            storedEntity.Description.ShouldBe(updatedEntity.Description);
            storedEntity.ImageUrls.ShouldBe(updatedEntity.ImageUrls);
            storedEntity.CreatedBy.ShouldBe(updatedEntity.CreatedBy);
            storedEntity.CreatedAt.ShouldNotBe(default);
            storedEntity.UpdatedAt.ShouldNotBe(default);
            storedEntity.UpdatedAt.ShouldBeGreaterThanOrEqualTo(storedEntity.CreatedAt);
        }
    }

    [Fact]
    public void Update_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var updatedEntity = new ClubDto
        {
            Id = -1000,
            Name = "Ne postoji klub",
            Description = "Neki opis",
            CreatedBy = 1,
            ImageUrls = new List<string> { "nema.jpg" }
        };

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Update(updatedEntity.Id, updatedEntity));
    }

    [Fact]
    public void Deletes()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        var newEntity = new ClubDto
        {
            Name = "Klub za brisanje",
            Description = "Opis kluba za brisanje",
            CreatedBy = 1,
            ImageUrls = new List<string> { "obrisi.jpg" }
        };

        // Prvo kreiramo klub koji ćemo kasnije brisati
        var createResult = (controller.Create(newEntity).Result as OkObjectResult)?.Value as ClubDto;
        createResult.ShouldNotBeNull();
        createResult.Id.ShouldNotBe(0);

        var idToDelete = createResult.Id;

        // Act - brišemo taj klub
        var deleteResult = controller.Delete(idToDelete) as OkResult;

        // Assert - Response
        deleteResult.ShouldNotBeNull();
        deleteResult.StatusCode.ShouldBe(200);

        // Assert - Database
        var stored = dbContext.Clubs.FirstOrDefault(c => c.Id == idToDelete);
        stored.ShouldBeNull();
    }

    [Fact]
    public void Delete_fails_invalid_id()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act & Assert
        Should.Throw<NotFoundException>(() => controller.Delete(-1000));
    }

    [Fact]
    public void Accept_membership_request_creates_notification()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();

        // owner = -21, tourist = -22 (može biti bilo koji ID, nema FK ka Person)
        const long ownerPersonId = 1;
        const long touristPersonId = -22;

        var ownerController = new ClubsController(clubService)
        {
            ControllerContext = BuildContext(ownerPersonId.ToString())
        };

        var touristController = new ClubsController(clubService)
        {
            ControllerContext = BuildContext(touristPersonId.ToString())
        };

        // 1) Owner kreira klub
        var createDto = new ClubDto
        {
            Name = "Klub za notifikacije",
            Description = "Test notifikacija",
            ImageUrls = new List<string> { "notif.jpg" }
        };

        var createdResult =
            (ownerController.Create(createDto).Result as OkObjectResult)?.Value as ClubDto;

        createdResult.ShouldNotBeNull();
        var clubId = createdResult.Id;
        clubId.ShouldNotBe(0);

        // 2) Turista šalje zahtev za učlanjenje
        var joinResult = touristController.RequestMembership(clubId) as OkObjectResult;
        joinResult.ShouldNotBeNull();
        joinResult.StatusCode.ShouldBe(200);

        // 3) Owner prihvata zahtev
        var acceptResult = ownerController.AcceptMembershipRequest(clubId, touristPersonId) as OkObjectResult;
        acceptResult.ShouldNotBeNull();
        acceptResult.StatusCode.ShouldBe(200);

        // 4) Assert – Notification u bazi
        var notifications = dbContext.Set<Notification>()
            .Where(n => n.UserId == touristPersonId && n.ClubId == clubId)
            .ToList();

        notifications.Count.ShouldBe(1);
        var notification = notifications.Single();
        notification.IsRead.ShouldBeFalse();
        notification.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public void Reject_membership_request_creates_notification()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var clubService = scope.ServiceProvider.GetRequiredService<IClubService>();

        const long ownerPersonId = 1;
        const long touristPersonId = -23; 

        var ownerController = new ClubsController(clubService)
        {
            ControllerContext = BuildContext(ownerPersonId.ToString())
        };

        var touristController = new ClubsController(clubService)
        {
            ControllerContext = BuildContext(touristPersonId.ToString())
        };

        // 1) Owner kreira klub
        var createDto = new ClubDto
        {
            Name = "Klub za reject notifikacije",
            Description = "Test reject notifikacija",
            ImageUrls = new List<string> { "notif-reject.jpg" }
        };

        var createdResult =
            (ownerController.Create(createDto).Result as OkObjectResult)?.Value as ClubDto;

        createdResult.ShouldNotBeNull();
        var clubId = createdResult.Id;
        clubId.ShouldNotBe(0);

        // 2) Turista šalje zahtev za učlanjenje
        var joinResult = touristController.RequestMembership(clubId) as OkObjectResult;
        joinResult.ShouldNotBeNull();
        joinResult.StatusCode.ShouldBe(200);

        // 3) Owner odbija zahtev
        var rejectResult = ownerController.RejectMembershipRequest(clubId, touristPersonId) as OkObjectResult;
        rejectResult.ShouldNotBeNull();
        rejectResult.StatusCode.ShouldBe(200);

        // 4) Assert – Notification u bazi
        var notifications = dbContext.Set<Notification>()
            .Where(n => n.UserId == touristPersonId && n.ClubId == clubId)
            .ToList();

        notifications.Count.ShouldBe(1);
        var notification = notifications.Single();
        notification.IsRead.ShouldBeFalse();
        notification.CreatedAt.ShouldNotBe(default);
        // Ako ti enum imena odgovaraju, može i:
        // notification.Type.ShouldBe(NotificationType.MembershipRejected);
    }


    private static ClubsController CreateController(IServiceScope scope)
    {
        return new ClubsController(scope.ServiceProvider.GetRequiredService<IClubService>())
        {
            ControllerContext = BuildContext("1")
        };
    }
}
