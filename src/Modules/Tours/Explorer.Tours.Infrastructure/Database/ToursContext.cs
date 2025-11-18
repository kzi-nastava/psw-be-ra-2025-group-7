using Explorer.Tours.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Explorer.Tours.Core.Domain.Entities;


namespace Explorer.Tours.Infrastructure.Database;

public class ToursContext : DbContext
{
    public DbSet<Equipment> Equipment { get; set; }
    public DbSet<Quiz> Quizzes { get; set; }
    public DbSet<Question> Questions { get; set; }


    public ToursContext(DbContextOptions<ToursContext> options) : base(options) {}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("tours");

        //modelBuilder.Entity<Quiz>()
        //.HasMany(q => q.Questions)
        //.WithOne(q => q.Quiz)
        //.HasForeignKey(q => q.QuizId);

        //modelBuilder.Entity<Question>()
        //    .HasMany(q => q.Options)
        //    .WithOne(o => o.Question)
        //    .HasForeignKey(o => o.QuestionId);
    }
}