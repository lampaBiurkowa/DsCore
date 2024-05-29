using DsCore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DsCore.Api.Infrastructure;

public class PaymentEntityTypeConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.Property(x => x.CurrencyId).IsRequired();
        builder.Property(x => x.UserGuid).IsRequired();
        builder.Property(x => x.Value).IsRequired();
    }
}
