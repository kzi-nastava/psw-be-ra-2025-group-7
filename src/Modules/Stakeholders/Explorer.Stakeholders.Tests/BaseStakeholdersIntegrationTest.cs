using Explorer.BuildingBlocks.Tests;
using Explorer.API.Controllers.Administrator;


namespace Explorer.Stakeholders.Tests;

public class BaseStakeholdersIntegrationTest : BaseWebIntegrationTest<StakeholdersTestFactory>
{
    public BaseStakeholdersIntegrationTest(StakeholdersTestFactory factory): base(factory) {}
}