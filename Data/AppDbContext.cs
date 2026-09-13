using FitnessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessManagement.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var user = modelBuilder.Entity<User>();

        user.ToTable("users");
        user.HasKey(x => x.Id);
        user.HasIndex(x => x.Email).IsUnique();

        user.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        user.Property(x => x.Email).HasMaxLength(255).IsRequired();
        user.Property(x => x.PhoneNumber).HasMaxLength(20);
        user.Property(x => x.CreatedAtUtc).HasPrecision(6);
        user.Property(x => x.UpdatedAtUtc).HasPrecision(6);
    }
}
