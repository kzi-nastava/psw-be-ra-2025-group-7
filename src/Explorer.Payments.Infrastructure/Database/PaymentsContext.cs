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

        // NEW (bundle)
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<BundleItem> BundleItems { get; set; }
        public DbSet<BundlePurchase> BundlePurchase { get; set; }

        // NEW (purchase notifications)
        public DbSet<PurchaseNotification> PurchaseNotifications { get; set; }

        // NEW (coupons)
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<PaymentRecord> PaymentRecords { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<SaleTour> SaleTours { get; set; }

        // NEW (crypto deposits)
        public DbSet<CryptoDepositRequest> CryptoDepositRequests { get; set; }

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

            // ===== Wallet konfiguracija =====
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

                builder.Property(w => w.SolanaWalletAddress)
                       .HasMaxLength(50);

                builder.HasIndex(w => w.UserId)
                       .IsUnique();

                builder.HasIndex(w => w.SolanaWalletAddress);
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

            // ===== PurchaseNotification konfiguracija (NEW) =====
            modelBuilder.Entity<PurchaseNotification>(builder =>
            {
                builder.ToTable("PurchaseNotifications");

                builder.HasKey(n => n.Id);

                builder.Property(n => n.TouristId)
                       .IsRequired();

                builder.Property(n => n.Message)
                       .IsRequired()
                       .HasMaxLength(500);

                builder.Property(n => n.CreatedAt)
                       .IsRequired()
                       .HasColumnType("timestamp with time zone");

                builder.Property(n => n.IsRead)
                       .IsRequired();

                builder.HasIndex(n => n.TouristId);
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

            modelBuilder.Entity<BundlePurchase>(b =>
            {
                b.ToTable("BundlePurchases");

                b.HasKey(x => x.Id);

                b.Property(x => x.TouristId)
                    .IsRequired();

                b.Property(x => x.BundleId)
                    .IsRequired();

                b.Property(x => x.Price)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(x => x.PurchasedAt)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.HasIndex(x => new { x.TouristId, x.BundleId })
                    .IsUnique();
            });

            // ===== Coupon konfiguracija =====
            modelBuilder.Entity<Coupon>(b =>
            {
                b.ToTable("Coupons");
                b.HasKey(c => c.Id);

                b.Property(c => c.Code)
                    .IsRequired()
                    .HasMaxLength(8);

                b.Property(c => c.DiscountPercentage)
                    .IsRequired();

                b.Property(c => c.ExpirationDate)
                    .HasColumnType("timestamp with time zone");

                b.Property(c => c.AuthorId)
                    .IsRequired();

                b.Property(c => c.TourId);

                b.Property(c => c.IsActive)
                    .IsRequired();

                b.HasIndex(c => c.Code)
                    .IsUnique();

                b.HasIndex(c => c.AuthorId);
            });

            // ===== PaymentRecord konfiguracija =====
            modelBuilder.Entity<PaymentRecord>(b =>
            {
                b.ToTable("PaymentRecords");
                b.HasKey(pr => pr.Id);

                b.Property(pr => pr.TouristId)
                    .IsRequired();

                b.Property(pr => pr.TourId);

                b.Property(pr => pr.BundleId);

                b.Property(pr => pr.OriginalPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(pr => pr.DiscountPercentage)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");

                b.Property(pr => pr.FinalPrice)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(pr => pr.PurchaseDate)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.Property(pr => pr.CouponCode)
                    .HasMaxLength(8);

                b.HasIndex(pr => pr.TouristId);
            });

            // ===== Sale konfiguracija =====
            modelBuilder.Entity<Sale>(b =>
            {
                b.ToTable("Sales");
                b.HasKey(s => s.Id);

                b.Property(s => s.AuthorId).IsRequired();

                b.Property(s => s.Start)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.Property(s => s.End)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.Property(s => s.DiscountPercentage).IsRequired();
                b.Property(s => s.Status).IsRequired();

                b.HasMany(s => s.SaleTours)
                    .WithOne(st => st.Sale)          // NAVIGACIJA
                    .HasForeignKey(st => st.SaleId)  // FK
                    .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<SaleTour>(b =>
            {
                b.ToTable("SaleTours");

                b.HasKey(st => new { st.SaleId, st.TourId });

                b.Property(st => st.TourId)
                    .IsRequired();
            });
            // ===== CryptoDepositRequest konfiguracija =====
            modelBuilder.Entity<CryptoDepositRequest>(b =>
            {
                b.ToTable("CryptoDepositRequests");
                b.HasKey(cdr => cdr.Id);

                b.Property(cdr => cdr.UserId)
                    .IsRequired();

                b.Property(cdr => cdr.TransactionId)
                    .IsRequired()
                    .HasMaxLength(100);

                b.Property(cdr => cdr.CryptoAmount)
                    .IsRequired()
                    .HasColumnType("decimal(18,8)");

                b.Property(cdr => cdr.CoinsAmount)
                    .IsRequired()
                    .HasColumnType("decimal(18,2)");

                b.Property(cdr => cdr.Status)
                    .IsRequired();

                b.Property(cdr => cdr.RequestedAt)
                    .IsRequired()
                    .HasColumnType("timestamp with time zone");

                b.Property(cdr => cdr.ConfirmedAt)
                    .HasColumnType("timestamp with time zone");

                b.Property(cdr => cdr.BlockchainExplorerUrl)
                    .HasMaxLength(500);

                b.Property(cdr => cdr.SenderWalletAddress)
                    .HasMaxLength(50);

                b.HasIndex(cdr => cdr.UserId);
                b.HasIndex(cdr => cdr.TransactionId)
                    .IsUnique();
                b.HasIndex(cdr => cdr.Status);
            });
        }
    }
}
