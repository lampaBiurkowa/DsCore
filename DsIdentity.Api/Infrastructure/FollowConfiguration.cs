using DsIdentity.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class FollowEntityTypeConfiguration : IEntityTypeConfiguration<Follow>
{
    public void Configure(EntityTypeBuilder<Follow> builder)
    {
        builder.HasOne(x => x.Followed).WithMany().OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(x => x.Follower).WithMany().OnDelete(DeleteBehavior.Restrict);
    }
}