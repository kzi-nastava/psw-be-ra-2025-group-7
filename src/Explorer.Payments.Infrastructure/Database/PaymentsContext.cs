using Explorer.Payments.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Payments.Infrastructure.Database
{
    public class PaymentsContext : DbContext
    {
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<PaymentNotification> PaymentNotifications { get; set; }

        // NEW
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<BundleItem> BundleItems { get; set; }

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

            // ===== PaymentNotification konfiguracija =====
            modelBuilder.Entity<PaymentNotification>(builder =>
            {
                builder.ToTable("PaymentNotifications");

                builder.HasKey(n => n.Id);

                builder.Property(n => n.UserId)
                       .IsRequired();

                builder.Property(n => n.Content)
                       .IsRequired()
                       .HasMaxLength(500);

                builder.Property(n => n.CreatedAt)
                       .IsRequired()
                       .HasColumnType("timestamp with time zone");

                builder.Property(n => n.IsRead)
                       .IsRequired();

                builder.HasIndex(n => n.UserId);
            });

            // ===== Bundle konfiguracija =====
            modelBuilder.Entity<Bundle>(b =>
            {
                b.ToTable("Bundles");
                b.HasKey(x => x.Id);

                b.Property(x => x.AuthorId).IsRequired();

                b.Property(x => x.Name)
                    .IsRequired()
                    .HasMaxLength(200);

                b.Property(x => x.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.Status)
                    .IsRequired();

                b.Property(x => x.CreatedAt)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.Property(x => x.UpdatedAt)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.HasIndex(x => x.AuthorId);

                b.HasMany(x => x.Items)
                    .WithOne()
                    .HasForeignKey("BundleId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BundleItem>(b =>
            {
                b.ToTable("BundleItems");
                b.HasKey(x => x.Id);

                b.Property(x => x.TourId).IsRequired();

                b.HasIndex("BundleId", nameof(BundleItem.TourId))
                    .IsUnique();
            });
        }
    }
}
