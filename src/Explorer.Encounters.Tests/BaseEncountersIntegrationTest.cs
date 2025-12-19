using Explorer.BuildingBlocks.Tests;
using Explorer.Encounters.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Tests;

public class BaseEncountersIntegrationTest : BaseWebIntegrationTest<EncountersTestFactory>
{
    public BaseEncountersIntegrationTest(EncountersTestFactory factory) : base(factory)
    {
        ResetEncounters();
    }

    private void ResetEncounters()
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<EncountersContext>();

        var sqlPath = Path.Combine(AppContext.BaseDirectory, "TestData", "a-delete.sql");
        var sql = File.ReadAllText(sqlPath);

        db.Database.ExecuteSqlRaw(sql);
    }
}
