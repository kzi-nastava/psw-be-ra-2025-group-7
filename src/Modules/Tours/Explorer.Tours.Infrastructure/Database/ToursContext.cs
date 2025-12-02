using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;


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


        modelBuilder.Entity<Tour>().HasKey(t => t.Id);
        modelBuilder.Entity<Tour>().Property(t => t.Tags).HasConversion(v => string.Join(',', v),
          
            
            v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

        modelBuilder.Entity<TourProblem>(b =>
        {
            b.ToTable("TourProblems");

            b.HasKey(tp => tp.Id);

            b.Property(tp => tp.TourId).IsRequired();
            b.Property(tp => tp.TouristId).IsRequired();

            b.Property(tp => tp.Category)
             .HasConversion<int>()
             .IsRequired();

            b.Property(tp => tp.Priority)
             .HasConversion<int>()
             .IsRequired();

            b.Property(tp => tp.Description)
             .IsRequired()
             .HasMaxLength(2000);

            b.Property(tp => tp.TimeReported)
             .IsRequired();

            b.Property(tp => tp.Status)
             .HasConversion<int>()
             .IsRequired();

            // Ignoriše javni immutable getter
            b.Ignore(tp => tp.Comments);

            // Mapa za private field _comments -> JSONB u Postgresu
            b.Property<List<TourProblemMessage>>("_comments")
             .HasColumnName("_comments")
             .HasColumnType("jsonb")
             .HasConversion(
                 v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                 v => JsonSerializer.Deserialize<List<TourProblemMessage>>(v, (JsonSerializerOptions?)null)
             );
        });
    }
}
