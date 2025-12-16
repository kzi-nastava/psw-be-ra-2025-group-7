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
    public DbSet<ClubJoinRequest> ClubJoinRequests { get; set; }
    public DbSet<ClubInvitation> ClubInvitations { get; set; }
    public DbSet<ClubMember> ClubMembers { get; set; }

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
        // Person configuration
        modelBuilder.Entity<Person>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<Person>(s => s.UserId);

        // UserProfile configuration
        modelBuilder.Entity<UserProfile>()
            .HasOne<User>()
            .WithOne()
            .HasForeignKey<UserProfile>(up => up.UserId);

        // Club configuration - EXPLICITLY configure Status
        modelBuilder.Entity<Club>()
            .HasKey(c => c.Id);

        modelBuilder.Entity<Club>()
            .Property(c => c.Status)
            .IsRequired()
            .HasConversion<int>(); // Ensure Status enum is stored as int

        modelBuilder.Entity<Club>()
            .HasMany(c => c.Members)
            .WithOne()
            .HasForeignKey("ClubId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Club>()
            .HasMany(c => c.JoinRequests)
            .WithOne()
            .HasForeignKey("ClubId")
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Club>()
            .HasMany(c => c.Invitations)
            .WithOne()
            .HasForeignKey("ClubId")
            .OnDelete(DeleteBehavior.Cascade);

        // ClubMember configuration
        modelBuilder.Entity<ClubMember>()
            .HasKey(m => m.Id);

        // ClubJoinRequest configuration
        modelBuilder.Entity<ClubJoinRequest>()
            .HasKey(r => r.Id);

        // ClubInvitation configuration
        modelBuilder.Entity<ClubInvitation>()
            .HasKey(i => i.Id);
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

        // Notification configuration (UNIFIED MODEL)
        modelBuilder.Entity<Notification>()
            .HasKey(n => n.Id);

        // UserId is required
        modelBuilder.Entity<Notification>()
            .Property(n => n.UserId)
            .IsRequired();

        // ClubId is NULLABLE (not all notifications are club-related)
        modelBuilder.Entity<Notification>()
            .Property(n => n.ClubId)
            .IsRequired(false); // ← FIX: Make nullable

        modelBuilder.Entity<Notification>()
            .Property(n => n.Type)
            .IsRequired()
            .HasConversion<int>();

        modelBuilder.Entity<Notification>()
            .Property(n => n.Content)
            .IsRequired();

        modelBuilder.Entity<Notification>()
            .Property(n => n.CreatedAt)
            .IsRequired();

        modelBuilder.Entity<Notification>()
            .Property(n => n.IsRead)
            .IsRequired();

        // Indexes
        modelBuilder.Entity<Notification>()
            .HasIndex(n => new { n.UserId, n.IsRead });
        modelBuilder.Entity<Notification>()
            .HasIndex(n => n.CreatedAt);

        // Foreign keys
        modelBuilder.Entity<Notification>()
            .HasOne<Person>()
            .WithMany()
            .HasForeignKey(n => n.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Notification>()
            .HasOne<Club>()
            .WithMany()
            .HasForeignKey(n => n.ClubId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne<FollowerMessage>()
            .WithMany()
            .HasForeignKey(n => n.SourceFollowerMessageId)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Notification>()
            .HasOne<ClubMessage>()
            .WithMany()
            .HasForeignKey(n => n.SourceClubMessageId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}