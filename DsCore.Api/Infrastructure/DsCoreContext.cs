using DibBase.Infrastructure;
using DsCore.Api.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DsCore.Infrastructure;

public class DsCoreContext : DibContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Follow> Follow { get; set; }
    public DbSet<Credentials> Credentials { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<Subscribtion> Subscribtion { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("Server=localhost;Database=DsCore;User=root;Password=root;", new MySqlServerVersion(new Version(5, 7, 0)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}