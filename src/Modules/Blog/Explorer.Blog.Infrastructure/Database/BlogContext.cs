using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}


    public DbSet<BlogPost> BlogPosts { get; set; } = null!;  //DbSet za BlogPost entitet
    public DbSet<BlogVote> BlogVotes { get; set; } = null!; 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("blog");

        // Konfiguracija za BlogPost
        modelBuilder.Entity<BlogPost>(b =>
        {
            b.ToTable("BlogPosts");
            b.HasKey(x => x.Id);

            b.Property(x => x.AuthorId).IsRequired();
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Description).IsRequired();
            b.Property(x => x.CreatedAt).IsRequired();

            b.Property(x => x.Status)
            .IsRequired()
            .HasConversion<int>();  

            b.Property(x => x.LastModifiedAt);

            // Images kao owned kolekcija (BlogImage)
            b.OwnsMany(x => x.Images, img =>
            {
                img.ToTable("BlogPostImages");
                img.WithOwner().HasForeignKey("BlogPostId");

                img.Property<int>("Id");            // shadow key za row u tabeli slika
                img.HasKey("Id");

                img.Property(i => i.Url).IsRequired();
                img.Property(i => i.Order).IsRequired();
            });
        });

        // Konfiguracija za BlogVote
        modelBuilder.Entity<BlogVote>(bv =>
        {
            bv.ToTable("BlogVotes");
            bv.HasKey(x => x.Id);

            bv.Property(x => x.UserId).IsRequired();
            bv.Property(x => x.Value).IsRequired();
            bv.Property(x => x.VotedAt).IsRequired();

            // Relacija: BlogPost 1 -> N BlogVotes
            bv.HasOne<BlogPost>()
             .WithMany(b => b.Votes)
             .HasForeignKey("BlogPostId")
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}