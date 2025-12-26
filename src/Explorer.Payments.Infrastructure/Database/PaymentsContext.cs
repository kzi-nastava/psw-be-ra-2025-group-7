using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public PaymentsContext(DbContextOptions<PaymentsContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("payments");

            // ===== ShoppingCart konfiguracija =====
            modelBuilder.Entity<ShoppingCart>(b =>
            {
                b.ToTable("ShoppingCarts");
                b.HasKey(sc => sc.Id);

                b.Property(sc => sc.TouristId)
                    .IsRequired();

                b.Property(sc => sc.TotalPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.HasIndex(sc => sc.TouristId)
                    .IsUnique();

                b.HasMany(sc => sc.Items)
                    .WithOne()
                    .HasForeignKey("ShoppingCartId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // ===== OrderItem konfiguracija =====
            modelBuilder.Entity<OrderItem>(b =>
            {
                b.ToTable("OrderItems");
                b.HasKey(oi => oi.Id);

                b.Property(oi => oi.TourId)
                    .IsRequired();

                b.Property(oi => oi.TourName)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(oi => oi.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");
            });

            // Wallet
            modelBuilder.Entity<Wallet>(builder =>
            {
                builder.ToTable("Wallets");

                builder.HasKey(w => w.Id);

                builder.Property(w => w.UserId)
                       .IsRequired();

                builder.Property(w => w.Balance)
                       .HasPrecision(18, 2)
                       .IsRequired();

                builder.Property(w => w.CreatedAt)
                       .IsRequired();

                builder.HasIndex(w => w.UserId)
                       .IsUnique();
            });

            // PaymentRecords
            modelBuilder.Entity<PaymentRecord>(b =>
            {
                b.ToTable("PaymentRecords");
                b.HasKey(pr => pr.Id);

                b.Property(pr => pr.UserId).IsRequired();
                b.Property(pr => pr.Amount).HasPrecision(18,2).IsRequired();
                b.Property(pr => pr.CreatedAt).IsRequired();
                b.Property(pr => pr.Description).IsRequired(false);
            });
        }
    }
}
