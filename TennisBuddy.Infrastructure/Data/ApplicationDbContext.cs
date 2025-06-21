using Microsoft.EntityFrameworkCore;
using TennisBuddy.Domain.Entities;
using System.Text.Json;

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

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .IsRequired()
                .HasMaxLength(36);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(e => e.ProfilePicture)
                .HasMaxLength(500);

            entity.Property(e => e.Location)
                .HasMaxLength(100);

            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(20);

            entity.Property(e => e.SkillLevel)
                .HasDefaultValue(null);

            // Configure PreferredPlayTimes as JSON (PostgreSQL native JSON support)
            entity.Property(e => e.PreferredPlayTimes)
                .HasColumnType("jsonb")
                .HasConversion(
                    v => v == null ? null : JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                    v => v == null ? null : JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions?)null));

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.Property(e => e.LastLoginAt)
                .HasDefaultValue(null);

            entity.Property(e => e.IsActive)
                .HasDefaultValue(true);

            // Indexes
            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.HasIndex(e => e.CreatedAt);

            entity.HasIndex(e => e.Location);

            entity.HasIndex(e => e.SkillLevel);
        });
    }
}