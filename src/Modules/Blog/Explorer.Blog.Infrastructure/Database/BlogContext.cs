using Explorer.Blog.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace Explorer.Blog.Infrastructure.Database;

public class BlogContext : DbContext
{
    public BlogContext(DbContextOptions<BlogContext> options) : base(options) {}


    public DbSet<BlogPost> BlogPosts { get; set; } = null!;  //DbSet za BlogPost entitet
    public DbSet<BlogComment> BlogComments { get; set; } = null!; //DbSet za BlogComment entitet
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

            b.OwnsMany(x => x.Comments, comment =>
            {
                comment.ToTable("BlogComments");
                comment.WithOwner().HasForeignKey("BlogPostId");

                comment.Property<long>("Id");
                comment.HasKey("Id");

                comment.Property(c => c.AuthorId).IsRequired();
                comment.Property(c => c.Text).IsRequired().HasMaxLength(1000);
                comment.Property(c => c.CreatedAt).IsRequired();
                comment.Property(c => c.LastModifiedAt);
            });
        });
    }
}