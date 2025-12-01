using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}


    public DbSet<BlogPost> BlogPosts { get; set; } = null!;  //DbSet za BlogPost entitet
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
    }
}