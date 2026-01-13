using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Public;
using Explorer.Stakeholders.Infrastructure.Database;

namespace Explorer.Stakeholders.Tests.Integration.UserProfile;

[Collection("Sequential")]
public class UserProfileTests : BaseStakeholdersIntegrationTest
{
    public UserProfileTests(StakeholdersTestFactory factory) : base(factory) { }

    [Fact]
    public void GetByUserId_Returns_Profile()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        // Act
        var result = service.GetByUserId(-13);

        // Assert
        result.ShouldNotBeNull();
        result.UserId.ShouldBe(-13);
        result.FirstName.ShouldBe("Sara");
        result.LastName.ShouldBe("Sarić");
        result.Biography.ShouldBe("Ljubitelj prirode i avanturista koji voli da deli svoja iskustva.");
        result.Motto.ShouldBe("Adventure is out there!");
    }


    [Fact]
    public void Create_Creates_Valid_Profile()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        var newProfile = new UserProfileDto
        {
            UserId = -23, // Steva - korisnik koji postoji ali nema profil
            FirstName = "Steva",
            LastName = "Stević",
            ProfilePicture = "https://example.com/steva.jpg",
            Biography = "Novi korisnik platforme koji voli planinarenje.",
            Motto = "The mountains are calling!"
        };

        var existingProfile = dbContext.UserProfiles.SingleOrDefault(up => up.UserId == -23);
        if (existingProfile != null)
        {
            dbContext.UserProfiles.Remove(existingProfile);
            dbContext.SaveChanges();
        }

        var result = service.Create(newProfile);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBeGreaterThan(0);
        result.UserId.ShouldBe(-23);
        result.FirstName.ShouldBe("Steva");
        result.LastName.ShouldBe("Stević");
        result.Biography.ShouldBe("Novi korisnik platforme koji voli planinarenje.");
        result.Motto.ShouldBe("The mountains are calling!");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProfile = dbContext.UserProfiles.FirstOrDefault(up => up.UserId == -23);
        storedProfile.ShouldNotBeNull();
        storedProfile.FirstName.ShouldBe("Steva");
    }

    [Fact]
    public void Create_Fails_For_Invalid_FirstName()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        var invalidProfile = new UserProfileDto
        {
            UserId = -23,
            FirstName = "", // Prazno ime
            LastName = "Test",
            Biography = "Test",
            Motto = "Test"
        };

        // Act & Assert
        Should.Throw<System.ArgumentException>(() => service.Create(invalidProfile));
    }

    [Fact]
    public void Create_Fails_For_Biography_Too_Long()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        var invalidProfile = new UserProfileDto
        {
            UserId = -23,
            FirstName = "Test",
            LastName = "User",
            Biography = new string('A', 501), // 501 karaktera - preko limita
            Motto = "Test"
        };

        // Act & Assert
        Should.Throw<System.ArgumentException>(() => service.Create(invalidProfile));
    }

    [Fact]
    public void Create_Fails_For_Motto_Too_Long()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        var invalidProfile = new UserProfileDto
        {
            UserId = -23,
            FirstName = "Test",
            LastName = "User",
            Biography = "Test",
            Motto = new string('A', 151) // 151 karaktera - preko limita
        };

        // Act & Assert
        Should.Throw<System.ArgumentException>(() => service.Create(invalidProfile));
    }

    [Fact]
    public void Update_Updates_Profile_Successfully()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        var updatedProfile = new UserProfileDto
        {
            Id = -21, // Perin postojeći profil
            UserId = -21,
            FirstName = "Petar", // Izmenjeno ime
            LastName = "Petrović", // Izmenjeno prezime
            ProfilePicture = "https://example.com/new-pera.jpg",
            Biography = "Ažurirana biografija nakon godina putovanja.",
            Motto = "Never stop exploring!"
        };

        // Act
        var result = service.Update(updatedProfile);

        // Assert - Response
        result.ShouldNotBeNull();
        result.Id.ShouldBe(-21);
        result.FirstName.ShouldBe("Petar");
        result.LastName.ShouldBe("Petrović");
        result.Biography.ShouldBe("Ažurirana biografija nakon godina putovanja.");
        result.Motto.ShouldBe("Never stop exploring!");

        // Assert - Database
        dbContext.ChangeTracker.Clear();
        var storedProfile = dbContext.UserProfiles.FirstOrDefault(up => up.UserId == -21);
        storedProfile.ShouldNotBeNull();
        storedProfile.FirstName.ShouldBe("Petar");
        storedProfile.LastName.ShouldBe("Petrović");
    }


    [Fact]
    public void Update_Fails_For_Invalid_Data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();

        var invalidProfile = new UserProfileDto
        {
            Id = -21,
            UserId = -21,
            FirstName = "", // Prazno ime
            LastName = "Test"
        };

        // Act & Assert
        Should.Throw<System.ArgumentException>(() => service.Update(invalidProfile));
    }

    [Fact]
    public void AddXP_Levels_Up_User_When_XP_Threshold_Reached()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var service = scope.ServiceProvider.GetRequiredService<IUserProfileService>();
        var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

        long touristUserId = -23; // Sara – tourist, već postoji u bazi

        // očistimo tracking da ne povuče keširane vrednosti
        dbContext.ChangeTracker.Clear();

        // Act
        service.AddXP(touristUserId, 150); // proizvoljna XP vrednost preko 100

        // Assert
        dbContext.ChangeTracker.Clear();
        var updatedProfile = dbContext.UserProfiles.First(up => up.UserId == touristUserId);

        updatedProfile.XP.ShouldBe(150);
        updatedProfile.Level.ShouldBe(2); // level up sa 1 → 2
    }

}