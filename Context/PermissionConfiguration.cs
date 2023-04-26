
using IdentityProvider.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityProvider.Context;

public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.HasKey(x=>x.Id);
        builder.HasIndex(x=>x.Name).IsUnique();
        builder.ToTable("Permission");

        builder.Property(x=>x.Name).HasColumnType("VARCHAR").IsRequired().HasMaxLength(20);
        builder.OwnsOne(x=>x.TimeStamp,nav =>{
            nav.Property(x=>x.CreatedAt);
            nav.Property(x=>x.UpdatedAt);
        });
    }
}