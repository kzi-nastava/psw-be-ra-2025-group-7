using Explorer.Notes.Core.Domain;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Explorer.Notes.Infrastructure.Database
{
    public class NotesContext : DbContext
    {
        public NotesContext(DbContextOptions<NotesContext> options) : base(options) { }
        public DbSet<Note> Notes { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("notes");

            modelBuilder.Entity<Note>(n =>
            {
                n.ToTable("Notes");
                n.HasKey(x => x.Id);

                n.Property(x => x.UserId).IsRequired();
                n.Property(x => x.Title).IsRequired().HasMaxLength(200);
                n.Property(x => x.Content).IsRequired().HasMaxLength(5000);
                n.Property(x => x.Type).IsRequired().HasConversion<int>();
                n.Property(x => x.IsPinned).IsRequired();
                n.Property(x => x.CreatedAt).IsRequired();
                n.Property(x => x.UpdatedAt).IsRequired();
                n.Property(x => x.TourId);

                n.Property(x => x.Tags)
                  .HasColumnType("jsonb")
                  .IsRequired();
            });
        }
    }
}