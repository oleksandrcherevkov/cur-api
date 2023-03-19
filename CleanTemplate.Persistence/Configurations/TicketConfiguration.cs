using CleanTemplate.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CleanTemplate.Persistence.Configurations;

internal class TicketConfiguration : IEntityTypeConfiguration<Ticket>
{
    public void Configure(EntityTypeBuilder<Ticket> builder)
    {
        builder.Property(e => e.Id);
        builder.HasKey(e => e.Id);

        builder.Property(e => e.BikeId);
        builder.Property(e => e.UserId);
        builder.Property(e => e.StartDate);

        builder
            .HasOne(e => e.Bike)
            .WithMany(e => e.Tickets)
            .HasForeignKey(e => e.BikeId);

        builder
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.UserId);
    }
}