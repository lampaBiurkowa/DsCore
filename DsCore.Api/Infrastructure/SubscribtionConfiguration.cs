using DsCore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DsCore.Api.Infrastructure;

public class CyclicFeeEntityTypeConfiguration : IEntityTypeConfiguration<CyclicFee>
{
    public void Configure(EntityTypeBuilder<CyclicFee> builder)
    {
        builder.Property(x => x.PaymentId).IsRequired();
        builder.Property(x => x.PaymentInterval).IsRequired();
    }
}
