using Microsoft.EntityFrameworkCore;
using TennisBuddy.Domain.Entities;

namespace TennisBuddy.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Only configure what Data Annotations can't handle
        modelBuilder.Entity<User>(entity =>
        {
            // Unique constraint
            entity.HasIndex(e => e.Email).IsUnique();

            // Performance indexes
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.Location);
            entity.HasIndex(e => e.SkillLevel);
            entity.HasIndex(e => e.IsActive);
            entity.HasIndex(e => e.IsOnline);

            // JSON conversion for complex types
            entity.Property(e => e.PreferredPlayTimes)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null));

            entity.Property(e => e.PreferredCourtTypes)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null));

            entity.Property(e => e.PlayingStyles)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<List<string>>(v, (System.Text.Json.JsonSerializerOptions?)null));

            entity.Property(e => e.CourtTypePreferences)
                .HasConversion(
                    v => v == null ? null : System.Text.Json.JsonSerializer.Serialize(v, (System.Text.Json.JsonSerializerOptions?)null),
                    v => v == null ? null : System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, int>>(v, (System.Text.Json.JsonSerializerOptions?)null));
        });
    }
}