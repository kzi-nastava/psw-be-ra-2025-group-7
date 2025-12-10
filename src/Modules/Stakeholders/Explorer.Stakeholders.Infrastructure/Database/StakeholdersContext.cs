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
    public DbSet<ClubJoinRequest> ClubJoinRequests { get; set; }
    public DbSet<ClubInvitation> ClubInvitations { get; set; }
    public DbSet<ClubMember> ClubMembers { get; set; }
    public DbSet<Notification> Notifications { get; set; }


    public StakeholdersContext(DbContextOptions<StakeholdersContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("stakeholders");

        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<Message>()
            .HasIndex(m => new { m.SentByUserId, m.SentToUserId, m.Id });

        modelBuilder.Entity<Notification>(builder =>
        {
            builder.ToTable("Notifications", "stakeholders");

            builder.HasKey(n => n.Id);

            builder.Property(n => n.TouristId)
                .IsRequired();

            builder.Property(n => n.ClubId)
                .IsRequired();

            builder.Property(n => n.Type)
                .IsRequired();

            builder.Property(n => n.CreatedAt)
                .IsRequired();

            builder.Property(n => n.IsRead)
                .IsRequired();
        });


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

        modelBuilder.Entity<ClubMember>()
            .HasKey(m => m.Id);

        modelBuilder.Entity<ClubJoinRequest>()
            .HasKey(r => r.Id);

        modelBuilder.Entity<ClubInvitation>()
            .HasKey(i => i.Id);
    }
}