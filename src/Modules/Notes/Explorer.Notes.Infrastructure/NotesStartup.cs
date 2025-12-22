using Explorer.Notes.API.Public;
using Explorer.Notes.Core.Domain.RepositoryInterfaces;
using Explorer.Notes.Core.Mappers;
using Explorer.Notes.Core.UseCases;
using Explorer.Notes.Infrastructure.Database;
using Explorer.Notes.Infrastructure.Database.Repositories;
using Explorer.BuildingBlocks.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Explorer.Notes.Infrastructure
{
    public static class NotesStartup
    {
        public static IServiceCollection ConfigureNotesModule(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<NoteProfile>();
            });

            SetupCore(services);
            SetupInfrastructure(services);
            return services;
        }

        private static void SetupCore(IServiceCollection services)
        {
            services.AddScoped<INoteService, NoteService>();
        }

        private static void SetupInfrastructure(IServiceCollection services)
        {
            var dataSourceBuilder = new NpgsqlDataSourceBuilder(DbConnectionStringBuilder.Build("notes"));
            dataSourceBuilder.EnableDynamicJson();
            var dataSource = dataSourceBuilder.Build();

            services.AddDbContext<NotesContext>(opt =>
                opt.UseNpgsql(dataSource,
                    x => x.MigrationsHistoryTable("__EFMigrationsHistory", "notes")));

            services.AddScoped<INoteRepository, NoteRepository>();
        }
    }
}