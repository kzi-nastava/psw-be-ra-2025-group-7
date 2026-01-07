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
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<TourJournal> TourJournals { get; set; }
        public DbSet<TouristEquipment> TouristEquipment { get; set; }
        public DbSet<Quiz> Quizzes { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<Facility> Facility { get; set; }
        public DbSet<TourProblem> TourProblems { get; set; }
        public DbSet<TourPurchaseToken> TourPurchaseTokens { get; set; }
        public DbSet<AnnualAward> AnnualAwards { get; set; }
        public DbSet<EnhancedReview> EnhancedReviews { get; set; }
        public DbSet<EnhancedReviewPro> EnhancedReviewPros { get; set; }
        public DbSet<EnhancedReviewCon> EnhancedReviewCons { get; set; }
        public DbSet<EnhancedReviewTag> EnhancedReviewTags { get; set; }
        public DbSet<EnhancedReviewImage> EnhancedReviewImages { get; set; }
        public DbSet<EnhancedReviewHelpfulVote> EnhancedReviewHelpfulVotes { get; set; }
        public DbSet<TourRequest> TourRequests { get; set; }
        public DbSet<TourRequestResponse> TourRequestResponses { get; set; }
        public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("tours");

            // ===== Notification konfiguracija =====
            modelBuilder.Entity<Notification>(b =>
            {
                b.ToTable("Notifications");
                b.HasKey(n => n.Id);
                b.Property(n => n.UserId).IsRequired();
                b.Property(n => n.Title).IsRequired();
                b.Property(n => n.Preview).IsRequired();
                b.Property(n => n.ProblemId).IsRequired();
                b.Property(n => n.CreatedAt).IsRequired();
                b.Property(n => n.IsRead).IsRequired();
                // No FK constraint - UserId is just a reference, not enforced by DB
            });

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

                // Configure foreign key relationship to Tour
                b.HasOne(tpt => tpt.Tour)
                    .WithMany()
                    .HasForeignKey(tpt => tpt.TourId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Create unique index to ensure one purchase per user per tour
                b.HasIndex(tpt => new { tpt.UserId, tpt.TourId })
                    .IsUnique();

                // Create index on UserId for faster queries
                b.HasIndex(tpt => tpt.UserId);
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

            modelBuilder.Entity<EnhancedReview>(b =>
            {
                b.ToTable("EnhancedReviews");
                b.HasKey(x => x.Id);

                b.Property(x => x.TextReview).HasMaxLength(2000);

                b.HasIndex(x => new { x.TourId, x.TouristId }).IsUnique();

                // Pros
                b.HasMany(x => x.Pros)
                    .WithOne()
                    .HasForeignKey(p => p.EnhancedReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Cons
                b.HasMany(x => x.Cons)
                    .WithOne()
                    .HasForeignKey(c => c.EnhancedReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Tags
                b.HasMany(x => x.SentimentTags)
                    .WithOne()
                    .HasForeignKey(t => t.EnhancedReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                // Images
                b.HasMany(x => x.Images)
                    .WithOne()
                    .HasForeignKey(i => i.EnhancedReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

                // HelpfulVotes
                b.HasMany(x => x.HelpfulVotes)
                    .WithOne()
                    .HasForeignKey(v => v.EnhancedReviewId)
                    .OnDelete(DeleteBehavior.Cascade);

            });

            modelBuilder.Entity<EnhancedReviewPro>(b =>
            {
                b.ToTable("EnhancedReviewPros");
                b.HasKey(x => x.Id);
                b.Property(x => x.Text).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<EnhancedReviewCon>(b =>
            {
                b.ToTable("EnhancedReviewCons");
                b.HasKey(x => x.Id);
                b.Property(x => x.Text).IsRequired().HasMaxLength(200);
            });

            modelBuilder.Entity<EnhancedReviewTag>(b =>
            {
                b.ToTable("EnhancedReviewTags");
                b.HasKey(x => x.Id);

                // enum kao int (default), može i eksplicitno:
                b.Property(x => x.Tag).IsRequired();
            });

            modelBuilder.Entity<EnhancedReviewImage>(b =>
            {
                b.ToTable("EnhancedReviewImages");
                b.HasKey(x => x.Id);

                b.Property(x => x.Url).IsRequired().HasMaxLength(2048);
                b.Property(x => x.SizeBytes).IsRequired();
            });

            modelBuilder.Entity<EnhancedReviewHelpfulVote>(b =>
            {
                b.ToTable("EnhancedReviewHelpfulVotes");
                b.HasKey(x => x.Id);

                b.HasIndex(x => new { x.EnhancedReviewId, x.TouristId }).IsUnique();
            });


            // Ostale entitete (Facility, TourProblem, ...) rade drugi u svojim karticama.

            // ===== TourProblem konfiguracija =====

            modelBuilder.Entity<Tour>().HasKey(t => t.Id);
            modelBuilder.Entity<Tour>().Property(t => t.Tags).HasConversion(v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList());

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

            //Tour Request
            modelBuilder.Entity<TourRequest>(b =>
            {
                b.ToTable("TourRequests");
                b.HasKey(tr => tr.Id);

                b.Property(tr => tr.TouristId).IsRequired();
                b.Property(tr => tr.Title).IsRequired().HasMaxLength(100);
                b.Property(tr => tr.Description).IsRequired();

                b.Property(tr => tr.Latitude).IsRequired(false);
                b.Property(tr => tr.Longitude).IsRequired(false);
                b.Property(tr => tr.Radius).IsRequired(false);

                b.Property(tr => tr.Budget).IsRequired().HasColumnType("decimal(18,2)");

                b.Property(tr => tr.PreferredDifficulty).IsRequired(false);
                b.Property(tr => tr.NumberOfParticipants).IsRequired();
                b.Property(tr => tr.PreferredDate).IsRequired(false);

                b.Property(tr => tr.Status).IsRequired();
                b.Property(tr => tr.CreatedAt).IsRequired();
                b.Property(tr => tr.ExpiresAt).IsRequired();
            });

            //Tour Request Response
            modelBuilder.Entity<TourRequestResponse>(b =>
            {
                b.ToTable("TourRequestResponses");
                b.HasKey(trr => trr.Id);

                b.Property(trr => trr.TourRequestId).IsRequired();
                b.Property(trr => trr.AuthorId).IsRequired();
                b.Property(trr => trr.ResponseType).IsRequired();

                b.Property(trr => trr.TourId).IsRequired(false);
                b.Property(trr => trr.ProposalDescription).IsRequired(false);

                b.Property(trr => trr.ProposedPrice).IsRequired().HasColumnType("decimal(18,2)");
                b.Property(trr => trr.Message).IsRequired(false).HasMaxLength(500);
                b.Property(trr => trr.Status).IsRequired();
                b.Property(trr => trr.CreatedAt).IsRequired();

                b.HasOne<TourRequest>().WithMany().HasForeignKey(trr => trr.TourRequestId).OnDelete(DeleteBehavior.Cascade);
                b.HasOne<Tour>().WithMany().HasForeignKey(trr => trr.TourId).OnDelete(DeleteBehavior.Restrict).IsRequired(false);
            });
        }
    }
}
