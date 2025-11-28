using Explorer.API.Controllers.Administrator;
using Explorer.Stakeholders.API.Dtos;
using Explorer.Stakeholders.API.Services;
using Explorer.Stakeholders.Core.Domain;
using Explorer.Stakeholders.Infrastructure.Database;
using Explorer.Stakeholders.Tests;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Shouldly;
using System.Linq;
using Xunit;




namespace Explorer.Stakeholders.Tests.Integration.Administration
{
    [Collection("Sequential")]
    public class AccountsTests : BaseStakeholdersIntegrationTest
    {
        public AccountsTests(StakeholdersTestFactory factory) : base(factory)
        {
        }

        [Fact]
        public void Get_all_accounts_returns_seed_data()
        {
            
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);

            
            var actionResult = controller.GetAll();   

            
            actionResult.ShouldNotBeNull();

            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var accounts = okResult.Value as System.Collections.Generic.IEnumerable<AccountDto>;
            accounts.ShouldNotBeNull();

            
            accounts!.Any(a => a.Id == -1 && a.Username == "test_author").ShouldBeTrue();
            accounts!.Any(a => a.Id == -2 && a.Username == "test_tourist").ShouldBeTrue();
            accounts!.Any(a => a.Id == -3 && a.Username == "test_admin").ShouldBeTrue();
        }

        [Fact]
        public void Block_author_account_sets_IsBlocked_true_and_persists_to_db()
        {
            
            using var scope = Factory.Services.CreateScope();
            var controller = CreateController(scope);
            var dbContext = scope.ServiceProvider.GetRequiredService<StakeholdersContext>();

            
            dbContext.Database.BeginTransaction();

            var actionResult = controller.Block(-1);   

            actionResult.ShouldNotBeNull();

            var okResult = actionResult.Result as OkObjectResult;
            okResult.ShouldNotBeNull();

            var accountDto = okResult.Value as AccountDto;
            accountDto.ShouldNotBeNull();
            accountDto!.Id.ShouldBe(-1);
            accountDto.IsBlocked.ShouldBeTrue();

            dbContext.ChangeTracker.Clear();
            var stored = dbContext.Accounts.FirstOrDefault(a => a.Id == -1);
            stored.ShouldNotBeNull();
            stored!.IsBlocked.ShouldBeTrue();
        }
        private AccountsController CreateController(IServiceScope scope)
        {
            return new AccountsController(scope.ServiceProvider.GetRequiredService<IAccountService>())
            {    
                ControllerContext = BuildContext("0")
            };
        }
    }
}
