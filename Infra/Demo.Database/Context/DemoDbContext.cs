using Demo.AppCore.Models;
using Microsoft.EntityFrameworkCore;

namespace Demo.Database.Context;

public class DemoDbContext : DbContext
{
    public DemoDbContext(DbContextOptions<DemoDbContext> options) : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Exam> Exams { get; set; }
    public DbSet<ExamResult> ExamResults { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasIndex(e => e.StudentNo).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();
        });

        modelBuilder.Entity<ExamResult>(entity =>
        {
            entity.HasOne(e => e.Student)
                .WithMany(s => s.ExamResults)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Exam)
                .WithMany(ex => ex.ExamResults)
                .HasForeignKey(e => e.ExamId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.StudentId, e.ExamId }).IsUnique();
        });

        base.OnModelCreating(modelBuilder);
    }
}