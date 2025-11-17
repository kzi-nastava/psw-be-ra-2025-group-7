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
}