using Explorer.Notes.Infrastructure.Database;
using Explorer.BuildingBlocks.Tests;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System;

namespace Explorer.Notes.Tests
{
    public class NotesTestFactory : BaseTestFactory<NotesContext>
    {
        protected override IServiceCollection ReplaceNeededDbContexts(IServiceCollection services)
        {
            var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<NotesContext>));
            services.Remove(descriptor!);

            services.AddDbContext<NotesContext>(SetupTestContextWithJson());

            return services;
        }
        private Action<DbContextOptionsBuilder> SetupTestContextWithJson()
        {
            return options =>
            {
                var connectionString = "Host=localhost;Database=explorer-v1-test;Username=postgres;Password=root;SearchPath=notes";

                var dataSourceBuilder = new NpgsqlDataSourceBuilder(connectionString);
                dataSourceBuilder.EnableDynamicJson();
                var dataSource = dataSourceBuilder.Build();

                options.UseNpgsql(dataSource, x => x.MigrationsHistoryTable("__EFMigrationsHistory", "notes"));
            };
        }
    }
}