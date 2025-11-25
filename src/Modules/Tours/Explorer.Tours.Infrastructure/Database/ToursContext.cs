using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Facility> Facility { get; set; }
    public DbSet<Tour> Tours { get; set; }
    public DbSet<TourJournal> TourJournals { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }


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
    }
}
