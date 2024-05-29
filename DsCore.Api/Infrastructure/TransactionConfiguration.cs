using DsCore.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DsCore.Api.Infrastructure;

public class TransactionEntityTypeConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder.Property(x => x.PaymentId).IsRequired();
    }
}
