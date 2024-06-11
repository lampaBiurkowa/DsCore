using DibBase.Infrastructure;
using DibBase.Options;
using DsCore.Api.Models;
using DsCore.Api.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Reflection;

namespace DsCore.Infrastructure;

public class DsCoreContext(IOptions<DsDbLibOptions> options) : DibContext
{
    public DbSet<User> User { get; set; }
    public DbSet<Follow> Follow { get; set; }
    public DbSet<Credentials> Credentials { get; set; }
    public DbSet<Currency> Currency { get; set; }
    public DbSet<CyclicFee> CyclicFee { get; set; }
    public DbSet<Payment> Payment { get; set; }
    public DbSet<Transaction> Transaction { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql($"Server={options.Value.Host};Database={options.Value.DatabaseName};User={options.Value.User};Password={options.Value.Password};",
        new MySqlServerVersion(new Version(5, 7, 0)));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}