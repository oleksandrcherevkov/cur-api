using CleanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTemplate.Persistence.Configurations;

internal class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.Property(e => e.Id);
        builder.HasKey(e => e.Id);

        builder
            .HasMany(e => e.Bikes)
            .WithOne(e => e.CurrentStation)
            .HasForeignKey(e => e.CurrentStationId);
    }
}