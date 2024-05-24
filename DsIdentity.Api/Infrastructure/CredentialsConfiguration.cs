using DsIdentity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CredentialsEntityTypeConfiguration : IEntityTypeConfiguration<Credentials>
{
    public void Configure(EntityTypeBuilder<Credentials> builder)
    {
        builder.HasIndex(x => x.UserId).IsUnique();
    }
}