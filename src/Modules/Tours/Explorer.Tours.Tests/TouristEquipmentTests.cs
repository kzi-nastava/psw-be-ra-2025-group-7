using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using Explorer.Tours.API.Dtos;
using Xunit;

namespace Explorer.Tours.Tests;

public class TouristEquipmentTests : BaseToursIntegrationTest
{
    private readonly HttpClient _client;

    public TouristEquipmentTests(ToursTestFactory factory) : base(factory)
    {
        // isti pattern kao u drugim testovima (EquipmentCommandTests itd.)
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_Returns_Test_Data()
    {
        // Arrange
        long touristId = -100;

        // Act
        var response = await _client.GetAsync($"/api/tourists/equipment/{touristId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<TouristEquipmentDto>>();

        Assert.NotNull(result);
        Assert.Equal(2, result!.Count); // dva reda iz b-insert-tourist-equipment.sql
    }

    [Fact]
    public async Task Update_Changes_Data_In_Database()
    {
        // Arrange
        var dto = new UpdateTouristEquipmentDto
        {
            TouristId = -100,
            EquipmentIds = new List<long> { -1 }   // hoćemo da ostane samo oprema -1
        };

        // Act
        var response = await _client.PutAsJsonAsync("/api/tourists/equipment", dto);

        // Assert HTTP OK
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var result = await response.Content.ReadFromJsonAsync<List<TouristEquipmentDto>>();
        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Equal(-1, result![0].EquipmentId);

        // Assert ponovnim čitanjem iz baze
        var dbResponse = await _client.GetAsync($"/api/tourists/equipment/{dto.TouristId}");
        var dbResult = await dbResponse.Content.ReadFromJsonAsync<List<TouristEquipmentDto>>();

        Assert.NotNull(dbResult);
        Assert.Single(dbResult!);
        Assert.Equal(-1, dbResult![0].EquipmentId);
    }
}
