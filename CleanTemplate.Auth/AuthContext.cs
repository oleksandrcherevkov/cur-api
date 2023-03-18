using CleanTemplate.Auth.Models;
using Microsoft.EntityFrameworkCore;

namespace CleanTemplate.Auth;

public class AuthContext<TUser, TRole> : DbContext
    where TUser : UserModel<TRole>
    where TRole : Enum
{
    public AuthContext()
    {
    }

    public AuthContext(DbContextOptions options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<TUser>(builder =>
        {
            builder.HasKey(e => e.Id);
            builder
                .HasIndex(e => e.EmailNormalized)
                .IsUnique();
        });
    }

    public DbSet<TUser> Users { get; set; } = null!;
}
