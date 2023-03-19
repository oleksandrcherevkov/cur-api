using CleanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTemplate.Persistence.Configurations;

internal class BikeConfiguration : IEntityTypeConfiguration<Bike>
{
    public void Configure(EntityTypeBuilder<Bike> builder)
    {
        builder
            .Property(b => b.Id)
            .HasField("_id")
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.HasKey(e => e.Id);

        builder
            .HasMany(e => e.Tickets)
            .WithOne(e => e.Bike)
            .HasForeignKey(e => e.BikeId);
    }
}