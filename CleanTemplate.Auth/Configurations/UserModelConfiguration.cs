using CleanTemplate.Auth.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTemplate.Auth;

internal class UserModelConfiguration<TRole> : IEntityTypeConfiguration<UserModel<TRole>>
    where TRole : Enum
{
    public void Configure(EntityTypeBuilder<UserModel<TRole>> builder)
    {
        builder.HasKey(e => e.Id);
        builder
            .HasIndex(e => e.EmailNormalized)
            .IsUnique();

    }
}