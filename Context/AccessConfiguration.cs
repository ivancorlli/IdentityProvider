using IdentityProvider.Entity;
using IdentityProvider.Enumerables;
using IdentityProvider.ValueObject;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IdentityProvider.Context;

public class AccessConfiguration : IEntityTypeConfiguration<Access>
{
    public void Configure(EntityTypeBuilder<Access> builder)
    {
        builder.Property<int>("Id").HasColumnName("Id");
        builder.HasKey("Id");
        builder.ToTable("Access");

        builder.Property(x=>x.PermissionId).HasColumnType("VARCHAR").IsRequired().HasMaxLength(255);
        builder.Property(x=>x.ResourceId).HasColumnType("VARCHAR").IsRequired().HasMaxLength(255);
        builder.Property(x=>x.ResourceType).HasColumnType("VARCHAR").IsRequired().HasMaxLength(255);
        builder.Property(x=>x.Status)
            .HasConversion(x => x.ToString(), x => (AccessStatus)Enum.Parse(typeof(UserStatus), x))
            .HasMaxLength(25)
            .IsRequired();
        builder.Property(x=>x.CreatedAt).HasColumnType("BIGINT");
        
        builder.HasOne<ApplicationUser>().WithMany(x=>x.Permissions).HasForeignKey(x=>x.ResourceId).OnDelete(DeleteBehavior.ClientCascade);
        builder.HasOne<Permission>().WithMany().HasForeignKey(x=>x.PermissionId).OnDelete(DeleteBehavior.ClientCascade);
    }
}