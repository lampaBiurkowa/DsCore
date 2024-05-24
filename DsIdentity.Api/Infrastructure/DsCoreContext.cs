using DibBase.Infrastructure;
using DsIdentity.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DsIdentity.Infrastructure;

public class DsIdentityContext : DibContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Follow> Follow { get; set; }
    public DbSet<Credentials> Credentials { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql("Server=localhost;Database=DsIdentity;User=root;Password=root;", new MySqlServerVersion(new Version(5, 7, 0)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}