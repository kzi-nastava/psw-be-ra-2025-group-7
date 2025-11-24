using Explorer.API.Controllers.Tourist;
using Explorer.Tours.API.Dtos;
using Explorer.Tours.API.Public.Tourist;
using Explorer.Tours.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Collections.Generic;

namespace Explorer.Tours.Tests;

[Collection("Sequential")]
public class TouristEquipmentTests : BaseToursIntegrationTest
{
    public TouristEquipmentTests(ToursTestFactory factory) : base(factory) { }
    [Fact]
    public void Get_Returns_Test_Data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();

        var controller = CreateController(scope);
        var touristId = -100;

        // Act
        var actionResult = controller.GetByTourist(touristId);
        var result = (actionResult.Result as ObjectResult)?.Value as List<TouristEquipmentDto>;

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2); // iz SQL insert skripte: -200 i -201
    }

    [Fact]
    public void Update_Changes_Data_In_Database()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var dbContext = scope.ServiceProvider.GetRequiredService<ToursContext>();

        var dto = new UpdateTouristEquipmentDto
        {
            TouristId = 1,
            EquipmentIds = new List<long> { 1 }   // želimo da ostane samo 1
        };

        // Act
        var actionResult = controller.Update(dto);
        var updateResult = (actionResult.Result as ObjectResult)?.Value as List<TouristEquipmentDto>;

        // Assert
        updateResult.ShouldNotBeNull();
        updateResult.Count.ShouldBe(1);
        updateResult[0].EquipmentId.ShouldBe(1);

        // Assert iz baze
        var stored = dbContext.TouristEquipment
                              .Where(x => x.TouristId == 1)
                              .ToList();

        stored.Count.ShouldBe(1);
        stored[0].EquipmentId.ShouldBe(1);
    }


    private static TouristEquipmentController CreateController(IServiceScope scope)
    {
        return new TouristEquipmentController(
            scope.ServiceProvider.GetRequiredService<ITouristEquipmentService>())
        {
            ControllerContext = BuildContext("-1")
        };
    }
}
