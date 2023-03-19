using CleanTemplate.Auth;
using CleanTemplate.Domain.Entities;
using CleanTemplate.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.Persistence;

public class MyDbContext : AuthContext<User, RoleEnum>
{
    public MyDbContext()
    {
    }

    public MyDbContext(DbContextOptions<MyDbContext> options)
        : base(options)
    {
    }

    public DbSet<Bike> Bikes { get; set; }
    public DbSet<Station> Stations { get; set; }
    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(MyDbContext).Assembly);
    }
}
