using Explorer.Stakeholders.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Stakeholders.Infrastructure.Database;

public class StakeholdersContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Person> People { get; set; }
    public DbSet<Review> Reviews { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Club> Clubs { get; set; }
    public DbSet<TourPreferences> TourPreferences { get; set; }
    public DbSet<UserProfile> UserProfiles { get; set; }
    
    // New follower system DbSets
    public DbSet<Follower> Followers { get; set; }
    public DbSet<FollowerMessage> FollowerMessages { get; set; }
    public DbSet<ClubMessage> ClubMessages { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Message>()
            .HasIndex(m => new { m.SentByUserId, m.SentToUserId, m.Id });

        ConfigureStakeholder(modelBuilder);
        ConfigureFollowerSystem(modelBuilder);
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

    private static void ConfigureFollowerSystem(ModelBuilder modelBuilder)
    {
        // Follower configuration
        modelBuilder.Entity<Follower>()
            .HasKey(f => f.Id);
        modelBuilder.Entity<Follower>()
            .HasIndex(f => new { f.FollowerId, f.FollowedId })
            .IsUnique();
        modelBuilder.Entity<Follower>()
            .HasIndex(f => f.FollowedId);
        modelBuilder.Entity<Follower>()
            .HasIndex(f => f.FollowerId);

        // FollowerMessage configuration
        modelBuilder.Entity<FollowerMessage>()
            .HasKey(fm => fm.Id);
        modelBuilder.Entity<FollowerMessage>()
            .Property(fm => fm.Content)
            .HasMaxLength(280)
            .IsRequired();
        modelBuilder.Entity<FollowerMessage>()
            .HasIndex(fm => fm.AuthorId);
        modelBuilder.Entity<FollowerMessage>()
            .HasIndex(fm => fm.CreatedAt);

        // ClubMessage configuration
        modelBuilder.Entity<ClubMessage>()
            .HasKey(cm => cm.Id);
        modelBuilder.Entity<ClubMessage>()
            .Property(cm => cm.Content)
            .HasMaxLength(280)
            .IsRequired();
        modelBuilder.Entity<ClubMessage>()
            .HasIndex(cm => cm.ClubId);
        modelBuilder.Entity<ClubMessage>()
            .HasIndex(cm => cm.AuthorId);
        modelBuilder.Entity<ClubMessage>()
            .HasIndex(cm => cm.CreatedAt);

        // Notification configuration (PLACEHOLDER)
        modelBuilder.Entity<Notification>()
            .HasKey(n => n.Id);
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead });
        modelBuilder.Entity<Notification>()
            .HasIndex(n => n.CreatedAt);
    }
}