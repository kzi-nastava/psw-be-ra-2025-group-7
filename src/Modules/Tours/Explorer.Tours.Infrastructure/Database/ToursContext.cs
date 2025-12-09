using System;
using System.Linq;
using Explorer.Tours.Core.Domain;
using Explorer.Tours.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Explorer.Tours.Infrastructure.Database
{
    public class ToursContext : DbContext
    {
        public DbSet<Equipment> Equipment { get; set; }
        public DbSet<Monument> Monuments { get; set; }
        public DbSet<Tour> Tours { get; set; }
        public DbSet<PublicPointRequest> PublicPointRequests { get; set; }
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

            // ===== Monument konfiguracija =====
            modelBuilder.Entity<Monument>(b =>
            {
                b.ToTable("Monuments");
                b.HasKey(m => m.Id);

                b.Property(m => m.Name).IsRequired();
                b.Property(m => m.Description).IsRequired();
                b.Property(m => m.YearOfCreation).IsRequired();
                b.Property(m => m.Status).IsRequired();
                b.Property(m => m.Latitude).IsRequired();
                b.Property(m => m.Longitude).IsRequired();
            });

            // ===== Tour konfiguracija =====
            modelBuilder.Entity<Tour>(b =>
            {
                b.ToTable("Tours");
                b.HasKey(t => t.Id);

                b.Property(t => t.Tags)
                    .HasConversion(
                        v => string.Join(',', v),
                        v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

                b.Property(t => t.PublishedAt).IsRequired(false);
                b.Property(t => t.ArchivedAt).IsRequired(false);

                // KARTICA 4 – LengthInKm kao obična double precision kolona
                b.Property(t => t.LengthInKm)
                    .IsRequired()
                    .HasColumnType("double precision");

                // KeyPoints konfiguracija
                b.OwnsMany(t => t.KeyPoints, kp =>
                {
                    kp.ToTable("KeyPoints");
                    kp.WithOwner().HasForeignKey("TourId");

                    kp.Property<long>("Id");
                    kp.HasKey("Id");

                    kp.Property(k => k.Latitude).IsRequired();
                    kp.Property(k => k.Longitude).IsRequired();

                    kp.Property(k => k.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                    kp.Property(k => k.Description)
                      .IsRequired()
                      .HasMaxLength(2000);

                    kp.Property(k => k.ImageUrl);
                    kp.Property(k => k.Secret).IsRequired();
                });

                // TourDuration konfiguracija
                b.OwnsMany(t => t.TourDurations, td =>
                {
                    td.ToTable("TourDurations");
                    td.WithOwner().HasForeignKey("TourId");

                    td.Property<long>("Id").ValueGeneratedOnAdd();
                    td.HasKey("Id");

                    td.Property(d => d.Type)
                      .HasColumnName("TransportType")
                      .HasConversion<string>()
                      .HasMaxLength(50)
                      .IsRequired();

                    td.Property(d => d.Minutes)
                      .HasColumnName("DurationInMinutes")
                      .IsRequired();

                    td.HasCheckConstraint("CK_TourDuration_Minutes_Positive", "\"DurationInMinutes\" > 0");
                });

                // REQUIRED EQUIPMENT many-to-many
                b.HasMany(t => t.RequiredEquipment)
                 .WithMany()
                 .UsingEntity<Dictionary<string, object>>(
                     "TourEquipment",
                     j => j.HasOne<Equipment>().WithMany()
                           .HasForeignKey("EquipmentId")
                           .OnDelete(DeleteBehavior.Cascade),
                     j => j.HasOne<Tour>().WithMany()
                           .HasForeignKey("TourId")
                           .OnDelete(DeleteBehavior.Cascade),
                     j =>
                     {
                         j.ToTable("TourEquipment");
                         j.HasKey("TourId", "EquipmentId");
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

                b.Property(te => te.TouristId).IsRequired();
                b.Property(te => te.EquipmentId).IsRequired();
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

            // ===== PublicPointRequest konfiguracija =====
            modelBuilder.Entity<PublicPointRequest>(b =>
            {
                b.ToTable("PublicPointRequests");

                b.HasKey(p => p.Id);

                b.Property(p => p.TourId).IsRequired();
                b.Property(p => p.KeyPointIndex).IsRequired();
                b.Property(p => p.AuthorId).IsRequired();
                b.Property(p => p.Status).IsRequired();

                b.Property(p => p.AdminComment).HasMaxLength(500);
                b.Property(p => p.CreatedAt).IsRequired();
                b.Property(p => p.ProcessedAt).IsRequired(false);
            });

            // ===== TourProblem konfiguracija =====
            modelBuilder.Entity<TourProblem>(b =>
            {
                b.ToTable("TourProblems");

                b.HasKey(tp => tp.Id);

                b.Property(tp => tp.TourId).IsRequired();
                b.Property(tp => tp.TouristId).IsRequired();

                b.Property(tp => tp.Category).HasConversion<int>().IsRequired();
                b.Property(tp => tp.Priority).HasConversion<int>().IsRequired();

                b.Property(tp => tp.Description).IsRequired().HasMaxLength(2000);
                b.Property(tp => tp.TimeReported).IsRequired();

                b.Property(tp => tp.Status).HasConversion<int>().IsRequired();

                b.Ignore(tp => tp.Comments);

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
}
