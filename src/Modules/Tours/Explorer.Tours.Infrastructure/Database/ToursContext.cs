using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain.Entities;


namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<TourJournal> TourJournals { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<Facility> Facility { get; set; }
    public DbSet<TourProblem> TourProblems { get; set; }
    public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
    
    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Postojeća konfiguracija
        modelBuilder.HasDefaultSchema("tours");

        modelBuilder.Entity<Tour>().HasKey(t => t.Id);
        modelBuilder.Entity<Tour>().Property(t => t.Tags).HasConversion(v => string.Join(',', v),
                                                                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<TourJournal>().HasKey(tj => tj.Id);
        modelBuilder.Entity<TourJournal>().Property(tj => tj.TouristId).IsRequired();
        modelBuilder.Entity<TourJournal>().Property(tj => tj.Name).IsRequired().HasMaxLength(200);
        modelBuilder.Entity<TourJournal>().Property(tj => tj.Country).IsRequired().HasMaxLength(100);
        modelBuilder.Entity<TourJournal>().Property(tj => tj.City).HasMaxLength(100);
        modelBuilder.Entity<TourJournal>().Property(tj => tj.CreatedAt).IsRequired();
        modelBuilder.Entity<TourJournal>().Property(tj => tj.Status).IsRequired();
        // ===== Equipment konfiguracija =====
        modelBuilder.Entity<Equipment>(b =>
        {
            b.ToTable("Equipment");

            b.HasKey(e => e.Id);

            b.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(200);

            b.Property(e => e.Description)
                .HasMaxLength(500);
        });

        // ===== TouristEquipment konfiguracija =====
        modelBuilder.Entity<TouristEquipment>(b =>
        {
            b.ToTable("TouristEquipment");

            b.HasKey(te => te.Id);

            b.Property(te => te.TouristId)
                .IsRequired();

            b.Property(te => te.EquipmentId)
                .IsRequired();
        });

        modelBuilder.Entity<Quiz>(b =>
        {
            b.ToTable("Quizzes");
            b.HasKey(q => q.Id);

            b.Property(q => q.Title).IsRequired();
            b.Property(q => q.AuthorId).IsRequired();
        });

        modelBuilder.Entity<Question>(b =>
        {
            b.ToTable("Questions"); 
            b.HasKey(q => q.Id);

            b.Property(q => q.Content).IsRequired();
            b.Property(q => q.AllowsMultipleCorrect).IsRequired();

            /*    b.HasOne<Quiz>()
                 .WithMany(q => q.Questions)
                 .HasForeignKey(q => q.QuizId);
            */
            b.HasOne(q => q.Quiz)
            .WithMany(qz => qz.Questions)
            .HasForeignKey(q => q.QuizId)
            .IsRequired();
        });

        modelBuilder.Entity<Option>(b =>
        {
            b.ToTable("Option");
            b.HasKey(o => o.Id);

            b.Property(o => o.Text).IsRequired();
            b.Property(o => o.IsCorrect).IsRequired();
            b.Property(o => o.Feedback).HasMaxLength(500);

       /*     b.HasOne<Question>()
             .WithMany(q => q.Options)
             .HasForeignKey(o => o.QuestionId); */
            b.HasOne(o => o.Question).WithMany(q => q.Options)
             .HasForeignKey(o => o.QuestionId)
             .IsRequired();
        });

        // ===== TourPurchaseToken konfiguracija =====
        modelBuilder.Entity<TourPurchaseToken>(b =>
        {
            b.ToTable("TourPurchaseTokens");

            b.HasKey(tpt => tpt.Id);

            b.Property(tpt => tpt.UserId)
                .IsRequired();

            b.Property(tpt => tpt.TourId)
                .IsRequired();

            b.Property(tpt => tpt.PurchaseDate)
                .IsRequired();

            // Create unique index to ensure one purchase per user per tour
            b.HasIndex(tpt => new { tpt.UserId, tpt.TourId })
                .IsUnique();
        });

        modelBuilder.Entity<Tour>().HasKey(t => t.Id);
        modelBuilder.Entity<Tour>().Property(t => t.Tags).HasConversion(v => string.Join(',', v),
                                                                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());
    }
}
