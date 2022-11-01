using Microsoft.EntityFrameworkCore;
using Xarajat.Bot.Entities;

namespace Xarajat.Bot.Context;

#pragma warning disable CS8618
public class XarajatDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<Room> Rooms { get; set; }
    public DbSet<Outlay> Outlays { get; set; }

    public XarajatDbContext(DbContextOptions options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //with configuration class
        OutLaysConfiguration.Configure(modelBuilder.Entity<Outlay>());

        //with configuration classes from assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(XarajatDbContext).Assembly);
    }
}