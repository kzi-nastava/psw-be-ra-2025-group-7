using System;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database
{
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
        public DbSet<AnnualAward> AnnualAwards { get; set; }
        public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tours");

            // ===== Tour konfiguracija (životni ciklus + tvoji KeyPoints) =====
            modelBuilder.Entity<Tour>(b =>
            {
                b.ToTable("Tours");
                b.HasKey(t => t.Id);

                b.Property(t => t.Tags)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

                // datumi iz development grane
                b.Property(t => t.PublishedAt).IsRequired(false);
                b.Property(t => t.ArchivedAt).IsRequired(false);

                // Kartica 3 – KeyPoints kao owned kolekcija
                b.OwnsMany(t => t.KeyPoints, kp =>
                {
                    kp.ToTable("KeyPoints");               // tabela: tours."KeyPoints"
                    kp.WithOwner().HasForeignKey("TourId");

                    // Shadow primarni ključ za red u KeyPoints tabeli
                    kp.Property<long>("Id");
                    kp.HasKey("Id");

                    kp.Property(k => k.Latitude)
                      .IsRequired();

                    kp.Property(k => k.Longitude)
                      .IsRequired();

                    kp.Property(k => k.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                    kp.Property(k => k.Description)
                      .IsRequired()
                      .HasMaxLength(2000);

                    kp.Property(k => k.ImageUrl);

                    kp.Property(k => k.Secret)
                      .IsRequired();

                    // TODO (drugi članovi): ovde kasnije mogu da dodaju npr. Order polje
                    // za redosled tačaka, ako im zatreba za svoje kartice.
                });
                b.OwnsMany(t => t.TourDurations, td =>
                {
                    td.ToTable("TourDurations");              // naziv tabele u bazi
                    td.WithOwner().HasForeignKey("TourId");   // FK ka Tour.Id

                    // Shadow primary key
                    td.Property<long>("Id");
                    td.HasKey("Id");

                    // Enum TravelType mapiramo kao string
                    td.Property(d => d.Type)
                      .HasColumnName("TransportType")        // da se poklapa sa tvojom kolonom
                      .HasConversion<string>()               // enum kao string u bazi
                      .IsRequired();

                    td.Property(d => d.Minutes)
                      .HasColumnName("DurationInMinutes")    // da se poklapa sa tvojom kolonom
                      .IsRequired();
                });
            });

            // ===== TourJournal konfiguracija =====
            modelBuilder.Entity<TourJournal>(b =>
            {
                b.ToTable("TourJournals");
                b.HasKey(tj => tj.Id);

                b.Property(tj => tj.TouristId).IsRequired();
                b.Property(tj => tj.Name).IsRequired().HasMaxLength(200);
                b.Property(tj => tj.Country).IsRequired().HasMaxLength(100);
                b.Property(tj => tj.City).HasMaxLength(100);
                b.Property(tj => tj.CreatedAt).IsRequired();
                b.Property(tj => tj.Status).IsRequired();
            });

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

            // ===== Quiz konfiguracija =====
            modelBuilder.Entity<Quiz>(b =>
            {
                b.ToTable("Quizzes");
                b.HasKey(q => q.Id);

                b.Property(q => q.Title).IsRequired();
                b.Property(q => q.AuthorId).IsRequired();
            });

            // ===== Question konfiguracija =====
            modelBuilder.Entity<Question>(b =>
            {
                b.ToTable("Questions");
                b.HasKey(q => q.Id);

                b.Property(q => q.Content).IsRequired();
                b.Property(q => q.AllowsMultipleCorrect).IsRequired();

                b.HasOne(q => q.Quiz)
                 .WithMany(qz => qz.Questions)
                 .HasForeignKey(q => q.QuizId)
                 .IsRequired();
            });

            // ===== Option konfiguracija =====
            modelBuilder.Entity<Option>(b =>
            {
                b.ToTable("Option");
                b.HasKey(o => o.Id);

                b.Property(o => o.Text).IsRequired();
                b.Property(o => o.IsCorrect).IsRequired();
                b.Property(o => o.Feedback).HasMaxLength(500);

                b.HasOne(o => o.Question)
                 .WithMany(q => q.Options)
                 .HasForeignKey(o => o.QuestionId)
                 .IsRequired();
            });

            // Ostale entitete (Facility, TourProblem, ...) rade drugi u svojim karticama.
        }
    }
}
