using DsCore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DsCore.Api.Infrastructure;

public class SubscribtionEntityTypeConfiguration : IEntityTypeConfiguration<Subscribtion>
{
    public void Configure(EntityTypeBuilder<Subscribtion> builder)
    {
        builder.Property(x => x.PaymentId).IsRequired();
        builder.Property(x => x.PaymentInterval).IsRequired();
    }
}
