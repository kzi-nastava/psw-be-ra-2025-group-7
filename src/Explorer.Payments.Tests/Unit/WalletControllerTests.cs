using Explorer.API.Controllers;
using Explorer.Payments.API.Dtos;
using Explorer.Payments.API.Public;
using Explorer.Payments.Infrastructure.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System;
using System.Linq;

namespace Explorer.Payments.Tests.Unit
{
    [Collection("Sequential")]
    public class WalletControllerTests : BasePaymentsIntegrationTest, IDisposable
    {
        private readonly long[] _testUserIds = new long[] { -2301, -2302 };

        public WalletControllerTests(PaymentsTestFactory factory) : base(factory) { }

        protected void SeedTestData(IServiceScope scope, long userId)
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Briši samo wallet za tog korisnika
            var existing = dbContext.Wallets.FirstOrDefault(w => w.UserId == userId);
            if (existing != null)
            {
                dbContext.Wallets.Remove(existing);
                dbContext.SaveChanges();
            }

            // Ubaci testni wallet
            dbContext.Wallets.Add(new Explorer.Payments.Core.Domain.Wallet(userId));
            dbContext.SaveChanges();
        }

        public void Dispose()
        {
            using var scope = Factory.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            // Briši samo testne wallet-e
            var walletsToRemove = dbContext.Wallets
                .Where(w => _testUserIds.Contains(w.UserId))
                .ToList();

            dbContext.Wallets.RemoveRange(walletsToRemove);
            dbContext.SaveChanges();
        }

        [Fact]
        public async Task AddFunds_Updates_Wallet_Balance_For_Admin()
        {
            using var scope = Factory.Services.CreateScope();
            long testUserId = -2301;
            SeedTestData(scope, testUserId);

            var controller = CreateAdminController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<PaymentsContext>();

            var depositDto = new WalletDepositDto
            {
                TouristUserId = testUserId,
                Amount = 50
            };

            var actionResult = await controller.AddFunds(depositDto);
            actionResult.ShouldBeOfType<OkResult>();

            var wallet = dbContext.Wallets.FirstOrDefault(x => x.UserId == testUserId);
            wallet.ShouldNotBeNull();
            wallet.Balance.ShouldBe(50);
        }

        [Fact]
        public void GetWalletById_Returns_Wallet_For_Admin()
        {
            using var scope = Factory.Services.CreateScope();
            long touristId = -2302;
            SeedTestData(scope, touristId);

            var controller = CreateAdminController(scope);

            var actionResult = controller.GetWalletByUserId(touristId);
            var wallet = (actionResult.Result as ObjectResult)?.Value as WalletDto;

            wallet.ShouldNotBeNull();
            wallet.Balance.ShouldBe(0);
        }

        private static WalletController CreateAdminController(IServiceScope scope)
        {
            return new WalletController(
                scope.ServiceProvider.GetRequiredService<IWalletService>())
            {
                ControllerContext = BuildContext("-1")
            };
        }
    }

}
