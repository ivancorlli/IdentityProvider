using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityProvider.Entity;
using IdentityProvider.Enumerables;

namespace IdentityProvider.Context;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.ToTable("Profile");

        // name
        builder.OwnsOne(x => x.Name, nav =>
        {
            nav.Property(x => x.FirstName).HasColumnType("VARCHAR").HasMaxLength(25).IsRequired();
            nav.Property(x => x.LastName).HasColumnType("VARCHAR").HasMaxLength(25).IsRequired();
            nav.Property(x => x.NormalizeName).HasColumnType("VARCHAR").HasMaxLength(50).IsRequired();
        });
        // gender
        builder.Property(x => x.Gender).HasDefaultValue(UserGender.None);
        // Birth
        builder.Property(x => x.Birth).HasColumnType("DATE");
        // Address
        builder.OwnsOne(x => x.Address, nav =>
        {
            nav.Property(x => x.Country).HasColumnType("VARCHAR").HasMaxLength(25);
            nav.Property(x => x.City).HasColumnType("VARCHAR").HasMaxLength(25);
            nav.Property(x => x.State).HasColumnType("VARCHAR").HasMaxLength(25);
            nav.Property(x => x.ZipCode).HasColumnType("VARCHAR").HasMaxLength(10);
        });
        // Contact
        builder.OwnsOne(x => x.EmergencyContact, nav =>
        {
            //Name
            nav.OwnsOne(x => x.Name, n =>
            {
                n.Property(x => x.FirstName).HasColumnType("VARCHAR").HasMaxLength(25);
                n.Property(x => x.LastName).HasColumnType("VARCHAR").HasMaxLength(25);
                n.Property(x => x.NormalizeName).HasColumnType("VARCHAR").HasMaxLength(50);
            });
            // Relation
            nav.Property(x => x.RelationShip).HasColumnType("VARCHAR").HasMaxLength(25);
            // Number
            nav.OwnsOne(x => x.Phone, n =>
            {
                n.Property(x => x.Number).HasColumnType("VARCHAR").HasMaxLength(15);
                n.Property(x => x.CountryCode).HasColumnType("VARCHAR").HasMaxLength(5);
                n.Property(x => x.CountryPrefix).HasColumnType("VARCHAR").HasMaxLength(5);
            });
        });
        //Bio
        builder.OwnsOne(x => x.Bio, nav =>
        {
            nav.Property(x => x.Value).HasColumnType("VARCHAR").HasMaxLength(350);
        });
        //Images

        builder.Property(x => x.ProfilePicture).HasColumnType("VARCHAR").HasMaxLength(250).IsRequired(false);
        builder.Property(x => x.LandscapePicture).HasColumnType("VARCHAR").HasMaxLength(250).IsRequired(false);
        // Medical
        builder.OwnsOne(x => x.Medical, nav =>
        {
            nav.Property(x => x.Aptitude)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100)
                ;
            nav.Property(x => x.Disabilities)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100);
        });
        //TimeStamps
        builder.OwnsOne(x => x.TimeStamp, nav =>
        {
            nav.Property(x => x.CreatedAt).HasColumnType("BIGINT");
            nav.Property(x => x.UpdatedAt).HasColumnType("BIGINT");
        });
    }
}