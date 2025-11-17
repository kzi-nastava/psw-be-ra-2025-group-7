using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain.Entities;
using Explorer.Stakeholders.Core.Domain;


namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<TouristEquipment> TouristEquipment { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }


    public ToursContext(DbContextOptions<ToursContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Postojeća konfiguracija
        modelBuilder.HasDefaultSchema("tours");

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
