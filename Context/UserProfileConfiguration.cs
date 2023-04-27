using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityProvider.Entity;
using IdentityProvider.Enumerables;

namespace IdentityProvider.Context;

public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfile>
{
    public void Configure(EntityTypeBuilder<UserProfile> builder)
    {
        builder.HasKey(x => x.Id);
        builder.ToTable("Profile");

        // name
        builder.OwnsOne(x => x.Name, nav =>
        {
            nav.Property(x => x.FirstName).HasColumnType("VARCHAR").HasMaxLength(25);
            nav.Property(x => x.LastName).HasColumnType("VARCHAR").HasMaxLength(25);
            nav.Property(x => x.NormalizeName).HasColumnType("VARCHAR").HasMaxLength(50);
        });
        // gender
        builder.Property(x => x.Gender)
            .HasConversion(x => x.ToString(), x => (UserGender)Enum.Parse(typeof(UserGender), x)
            );
        // Birth
        builder.Property(x => x.Birth)
            .HasColumnType("date");
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
            nav.Property(x => x.RelationShip)
                 .HasConversion(x => x.ToString(), x => (RelationShip)Enum.Parse(typeof(RelationShip), x));
            // Number
            nav.OwnsOne(x => x.Phone, n =>
            {
                n.Property(x => x.Number).HasColumnType("VARCHAR").HasMaxLength(15);
                n.Property(x => x.CountryCode).HasColumnType("VARCHAR").HasMaxLength(5);
                n.Property(x => x.CountryPrefix).HasColumnType("VARCHAR").HasMaxLength(5);
            });
        });
        //Images
        builder.OwnsOne(x=> x.Pictures,nav=>{
            nav.Property(x=>x.ProfilePicture).HasColumnType("VARCHAR").HasMaxLength(250);
        });
        //Bio
        builder.OwnsOne(x => x.Bio, nav =>
        {
            nav.Property(x => x.Value).HasColumnType("VARCHAR").HasMaxLength(350);
        });
        // Medical
        builder.OwnsOne(x => x.Medical, nav =>
        {
            nav.Property(x => x.Aptitude)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100)
                .HasDefaultValue(null)
                .IsRequired();
            nav.Property(x => x.Disabilities)
                .HasColumnType("VARCHAR")
                .HasMaxLength(100)
                .HasDefaultValue(null)
                .IsRequired();
        });
        //TimeStamps
        builder.OwnsOne(x => x.TimeStamp, nav =>
        {
            nav.Property(x => x.CreatedAt).HasColumnType("BIGINT");
            nav.Property(x => x.UpdatedAt).HasColumnType("BIGINT");
        });
    }
}