using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using Explorer.API.Controllers.Author;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Xunit;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Explorer.Payments.Tests.Integration;

[Collection("Sequential")]
public class SaleControllerTests : BasePaymentsIntegrationTest, IClassFixture<PaymentsTestFactory>
{
    public SaleControllerTests(PaymentsTestFactory factory) : base(factory)
    {
        // Seed data loaded automatically
    }

    private static SaleController CreateController(IServiceScope scope)
    {
        return new SaleController(
            scope.ServiceProvider.GetRequiredService<ISaleService>()
        )
        {
            ControllerContext = BuildContext("-11") // Author ID -11
        };
    }

    // --------------------------------------------------
    // CREATE
    // --------------------------------------------------

    [Fact]
    public void Can_create_sale_with_valid_data()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var newSale = new SaleDto
        {
            DiscountPercentage = 20,
            Start = DateTime.UtcNow.AddDays(1),
            End = DateTime.UtcNow.AddDays(7),
            TourIds = new List<long> { -1, -2 }
        };

        // Act
        var result = ((ObjectResult)controller.Create(newSale))?.Value as SaleDto;

        // Assert
        result.ShouldNotBeNull();
        result.AuthorId.ShouldBe(-11);
        result.DiscountPercentage.ShouldBe(20);
        result.Status.ShouldBe(SaleStatusDto.Draft);
        result.TourIds.Count.ShouldBe(2);
    }

    [Fact]
    public void Create_fails_without_tours()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalidSale = new SaleDto
        {
            DiscountPercentage = 20,
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddDays(5),
            TourIds = new List<long>()
        };

        Should.Throw<Exception>(() => controller.Create(invalidSale))
            .Message.ShouldBe("Sale must contain at least one tour.");
    }


    [Fact]
    public void Create_fails_with_invalid_discount()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalidSale = new SaleDto
        {
            DiscountPercentage = 150,
            Start = DateTime.UtcNow,
            End = DateTime.UtcNow.AddDays(5),
            TourIds = new List<long> { -1 }
        };

        Should.Throw<Exception>(() => controller.Create(invalidSale))
            .Message.ShouldBe("Invalid discount percentage.");
    }


    [Fact]
    public void Create_fails_when_end_before_start()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var invalidSale = new SaleDto
        {
            DiscountPercentage = 10,
            Start = DateTime.UtcNow.AddDays(5),
            End = DateTime.UtcNow,
            TourIds = new List<long> { -1 }
        };

        Should.Throw<Exception>(() => controller.Create(invalidSale))
            .Message.ShouldBe("End date must be after start date.");
    }


    // --------------------------------------------------
    // GET
    // --------------------------------------------------

    [Fact]
    public void Can_get_all_sales_for_author()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        var result = ((ObjectResult)controller.GetAll())?.Value as List<SaleDto>;

        result.ShouldNotBeNull();
        result.ShouldNotBeEmpty();
    }


    [Fact]
    public void Can_get_only_active_sales()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);

        // Act
        var activeSales = ((ObjectResult)controller.GetAllActive())?.Value as List<SaleDto>;

        // Assert
        activeSales.ShouldNotBeNull();
        activeSales.All(s => s.Status == SaleStatusDto.Active).ShouldBeTrue();
    }

    // --------------------------------------------------
    // ACTIVATE
    // --------------------------------------------------

    [Fact]
    public void Can_activate_sale_with_valid_duration()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var sale = context.Sales.First(s => s.AuthorId == -11);

        sale.Start = DateTime.UtcNow;
        sale.End = DateTime.UtcNow.AddDays(10);
        context.SaveChanges();

        context.ChangeTracker.Clear();  

        var result = ((ObjectResult)controller.Activate(sale.Id))?.Value as SaleDto;

        result.ShouldNotBeNull();
        result.Status.ShouldBe(SaleStatusDto.Active);

        context.ChangeTracker.Clear();
        var storedSale = context.Sales.Find(sale.Id);
        storedSale.Status.ShouldBe(Core.Domain.SaleStatus.Active);
    }


    [Fact]
    public void Activate_fails_if_sale_lasts_more_than_14_days()
    {
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var sale = context.Sales.First(s => s.AuthorId == -11);

        sale.Start = DateTime.UtcNow;
        sale.End = DateTime.UtcNow.AddDays(20);
        context.SaveChanges();

        Should.Throw<Exception>(() => controller.Activate(sale.Id))
            .Message.ShouldBe("Sale cannot last more than 14 days.");
    }

    // --------------------------------------------------
    // ARCHIVE
    // --------------------------------------------------

    [Fact]
    public void Can_archive_sale()
    {
        // Arrange
        using var scope = Factory.Services.CreateScope();
        var controller = CreateController(scope);
        var context = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

        var sale = context.Sales.First(s => s.AuthorId == -11);

        context.ChangeTracker.Clear();   

        // Act
        var result = controller.Archive(sale.Id);

        // Assert
        result.ShouldBeOfType<OkResult>();

        context.ChangeTracker.Clear();
        var storedSale = context.Sales.Find(sale.Id);
        storedSale.Status.ShouldBe(Core.Domain.SaleStatus.Expired);
    }

}
