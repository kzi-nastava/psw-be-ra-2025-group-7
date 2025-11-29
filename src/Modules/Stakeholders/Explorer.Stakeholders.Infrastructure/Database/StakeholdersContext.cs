using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Review> Reviews { get; set; }

    public DbSet<Message> Messages { get; set; }
    public DbSet<Monument> Monuments { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Message>()
            .HasIndex(m => new { m.SentByUserId, m.SentToUserId, m.Id });

        ConfigureStakeholder(modelBuilder);
    }

    private static void ConfigureStakeholder(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);

        modelBuilder.Entity<UserProfile>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId);
    }
}