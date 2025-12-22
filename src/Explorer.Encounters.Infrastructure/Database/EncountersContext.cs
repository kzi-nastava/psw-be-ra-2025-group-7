using Explorer.Encounters.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Explorer.Encounters.Infrastructure.Database;

public class EncountersContext : DbContext
{
    public EncountersContext(DbContextOptions<EncountersContext> options) : base(options) { }

    public DbSet<Encounter> Encounters { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("encounters");

        modelBuilder.Entity<Encounter>(e =>
        {
            e.ToTable("Encounters");
            e.HasKey(x => x.Id);

            e.Property(x => x.CreatorId).IsRequired();

            e.Property(x => x.Name)
                .IsRequired()
                .HasMaxLength(200);

            e.Property(x => x.Description)
                .IsRequired();

            e.Property(x => x.Xp)
                .IsRequired();

            e.Property(x => x.Status)
                .IsRequired()
                .HasConversion<int>();

            e.Property(x => x.Type)
                .IsRequired()
                .HasConversion<int>();

            // GeoLocation (VALUE OBJECT)
            e.OwnsOne(x => x.Location, loc =>
            {
                loc.ToJson();

                loc.Property(p => p.Latitude).HasColumnName("latitude");
                loc.Property(p => p.Longitude).HasColumnName("longitude");
            });

            // HiddenLocationEncounter (VALUE OBJECT, OPTIONAL)
            e.OwnsOne(x => x.HiddenLocationDetails, hl =>
            {
                hl.ToJson();

                hl.Property(p => p.ImageUrl)
                    .HasColumnName("imageUrl")
                    .IsRequired();

                hl.Property(p => p.ActivationRadiusMeters)
                    .HasColumnName("activationRadiusMeters")
                    .IsRequired();

                hl.OwnsOne(p => p.ActivationLocation, al =>
                {
                    al.Property(p => p.Latitude).HasColumnName("activationLatitude");
                    al.Property(p => p.Longitude).HasColumnName("activationLongitude");
                });

                hl.OwnsOne(p => p.PhotoLocation, pl =>
                {
                    pl.Property(p => p.Latitude).HasColumnName("photoLatitude");
                    pl.Property(p => p.Longitude).HasColumnName("photoLongitude");
                });
            });
        });
    }



}


