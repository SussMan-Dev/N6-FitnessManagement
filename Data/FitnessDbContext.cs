using FitnessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessManagement.Data;

public class FitnessDbContext : DbContext
{
    public FitnessDbContext(DbContextOptions<FitnessDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
}