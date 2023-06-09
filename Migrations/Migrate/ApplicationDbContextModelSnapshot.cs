﻿// <auto-generated />
using System;
using IdentityProvider.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace IdentityProvider.Migrations.Migrate
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("IdentityProvider.Entity.ApplicationRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("Type")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("VARCHAR");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("IdentityProvider.Entity.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("longtext");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("IsAuthenticatedExternaly")
                        .HasColumnType("tinyint(1)");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("longtext");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("longtext");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("tinyint(1)");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("varchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("IdentityProvider.Entity.Permission", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("VARCHAR");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("Permission", (string)null);
                });

            modelBuilder.Entity("IdentityProvider.Entity.UserProfile", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime?>("Birth")
                        .HasColumnType("DATE");

                    b.Property<byte>("Gender")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint unsigned")
                        .HasDefaultValue((byte)0);

                    b.Property<string>("LandscapePicture")
                        .HasMaxLength(250)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("ProfilePicture")
                        .HasMaxLength(250)
                        .HasColumnType("VARCHAR");

                    b.HasKey("UserId");

                    b.ToTable("Profile", (string)null);
                });

            modelBuilder.Entity("IdentityProvider.ValueObject.Access", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    b.Property<long>("CreatedAt")
                        .HasColumnType("BIGINT");

                    b.Property<string>("PermissionId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("ResourceId")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("ResourceType")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("VARCHAR");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasMaxLength(25)
                        .HasColumnType("varchar(25)");

                    b.HasKey("Id");

                    b.HasIndex("PermissionId");

                    b.HasIndex("ResourceId");

                    b.ToTable("Access", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("ClaimType")
                        .HasColumnType("longtext");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Name")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("Value")
                        .HasColumnType("longtext");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ClientId")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("ClientSecret")
                        .HasColumnType("longtext");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("ConsentType")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("DisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("DisplayNames")
                        .HasColumnType("longtext");

                    b.Property<string>("Permissions")
                        .HasColumnType("longtext");

                    b.Property<string>("PostLogoutRedirectUris")
                        .HasColumnType("longtext");

                    b.Property<string>("Properties")
                        .HasColumnType("longtext");

                    b.Property<string>("RedirectUris")
                        .HasColumnType("longtext");

                    b.Property<string>("Requirements")
                        .HasColumnType("longtext");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ClientId")
                        .IsUnique();

                    b.ToTable("OpenIddictApplications", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ApplicationId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Properties")
                        .HasColumnType("longtext");

                    b.Property<string>("Scopes")
                        .HasColumnType("longtext");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Subject")
                        .HasMaxLength(400)
                        .HasColumnType("varchar(400)");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("ApplicationId", "Status", "Subject", "Type");

                    b.ToTable("OpenIddictAuthorizations", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreScope", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Description")
                        .HasColumnType("longtext");

                    b.Property<string>("Descriptions")
                        .HasColumnType("longtext");

                    b.Property<string>("DisplayName")
                        .HasColumnType("longtext");

                    b.Property<string>("DisplayNames")
                        .HasColumnType("longtext");

                    b.Property<string>("Name")
                        .HasMaxLength(200)
                        .HasColumnType("varchar(200)");

                    b.Property<string>("Properties")
                        .HasColumnType("longtext");

                    b.Property<string>("Resources")
                        .HasColumnType("longtext");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique();

                    b.ToTable("OpenIddictScopes", (string)null);
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreToken", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ApplicationId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("AuthorizationId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("ConcurrencyToken")
                        .IsConcurrencyToken()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<DateTime?>("CreationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<DateTime?>("ExpirationDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("Payload")
                        .HasColumnType("longtext");

                    b.Property<string>("Properties")
                        .HasColumnType("longtext");

                    b.Property<DateTime?>("RedemptionDate")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("ReferenceId")
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("Status")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("Subject")
                        .HasMaxLength(400)
                        .HasColumnType("varchar(400)");

                    b.Property<string>("Type")
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorizationId");

                    b.HasIndex("ReferenceId")
                        .IsUnique();

                    b.HasIndex("ApplicationId", "Status", "Subject", "Type");

                    b.ToTable("OpenIddictTokens", (string)null);
                });

            modelBuilder.Entity("IdentityProvider.Entity.Permission", b =>
                {
                    b.OwnsOne("IdentityProvider.ValueObject.TimeStamp", "TimeStamp", b1 =>
                        {
                            b1.Property<string>("PermissionId")
                                .HasColumnType("varchar(255)");

                            b1.Property<long>("CreatedAt")
                                .HasColumnType("bigint");

                            b1.Property<long>("UpdatedAt")
                                .HasColumnType("bigint");

                            b1.HasKey("PermissionId");

                            b1.ToTable("Permission");

                            b1.WithOwner()
                                .HasForeignKey("PermissionId");
                        });

                    b.Navigation("TimeStamp")
                        .IsRequired();
                });

            modelBuilder.Entity("IdentityProvider.Entity.UserProfile", b =>
                {
                    b.HasOne("IdentityProvider.Entity.ApplicationUser", null)
                        .WithOne("Profile")
                        .HasForeignKey("IdentityProvider.Entity.UserProfile", "UserId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.OwnsOne("IdentityProvider.ValueObject.PersonName", "Name", b1 =>
                        {
                            b1.Property<string>("UserProfileUserId")
                                .HasColumnType("varchar(255)");

                            b1.Property<string>("FirstName")
                                .IsRequired()
                                .HasMaxLength(25)
                                .HasColumnType("VARCHAR");

                            b1.Property<string>("LastName")
                                .IsRequired()
                                .HasMaxLength(25)
                                .HasColumnType("VARCHAR");

                            b1.Property<string>("NormalizeName")
                                .IsRequired()
                                .HasMaxLength(50)
                                .HasColumnType("VARCHAR");

                            b1.HasKey("UserProfileUserId");

                            b1.ToTable("Profile");

                            b1.WithOwner()
                                .HasForeignKey("UserProfileUserId");
                        });

                    b.OwnsOne("IdentityProvider.ValueObject.TimeStamp", "TimeStamp", b1 =>
                        {
                            b1.Property<string>("UserProfileUserId")
                                .HasColumnType("varchar(255)");

                            b1.Property<long>("CreatedAt")
                                .HasColumnType("BIGINT");

                            b1.Property<long>("UpdatedAt")
                                .HasColumnType("BIGINT");

                            b1.HasKey("UserProfileUserId");

                            b1.ToTable("Profile");

                            b1.WithOwner()
                                .HasForeignKey("UserProfileUserId");
                        });

                    b.OwnsOne("IdentityProvider.ValueObject.Address", "Address", b1 =>
                        {
                            b1.Property<string>("UserProfileUserId")
                                .HasColumnType("varchar(255)");

                            b1.Property<string>("City")
                                .IsRequired()
                                .HasMaxLength(25)
                                .HasColumnType("VARCHAR");

                            b1.Property<string>("Country")
                                .IsRequired()
                                .HasMaxLength(25)
                                .HasColumnType("VARCHAR");

                            b1.Property<string>("State")
                                .IsRequired()
                                .HasMaxLength(25)
                                .HasColumnType("VARCHAR");

                            b1.Property<string>("Street")
                                .HasColumnType("longtext");

                            b1.Property<int?>("StreetNumber")
                                .HasColumnType("int");

                            b1.Property<string>("ZipCode")
                                .IsRequired()
                                .HasMaxLength(10)
                                .HasColumnType("VARCHAR");

                            b1.HasKey("UserProfileUserId");

                            b1.ToTable("Profile");

                            b1.WithOwner()
                                .HasForeignKey("UserProfileUserId");
                        });

                    b.OwnsOne("IdentityProvider.ValueObject.Bio", "Bio", b1 =>
                        {
                            b1.Property<string>("UserProfileUserId")
                                .HasColumnType("varchar(255)");

                            b1.Property<string>("Value")
                                .IsRequired()
                                .HasMaxLength(350)
                                .HasColumnType("VARCHAR");

                            b1.HasKey("UserProfileUserId");

                            b1.ToTable("Profile");

                            b1.WithOwner()
                                .HasForeignKey("UserProfileUserId");
                        });

                    b.OwnsOne("IdentityProvider.ValueObject.EmergencyContact", "EmergencyContact", b1 =>
                        {
                            b1.Property<string>("UserProfileUserId")
                                .HasColumnType("varchar(255)");

                            b1.Property<string>("RelationShip")
                                .IsRequired()
                                .HasMaxLength(25)
                                .HasColumnType("VARCHAR");

                            b1.HasKey("UserProfileUserId");

                            b1.ToTable("Profile");

                            b1.WithOwner()
                                .HasForeignKey("UserProfileUserId");

                            b1.OwnsOne("IdentityProvider.ValueObject.ContactPhone", "Phone", b2 =>
                                {
                                    b2.Property<string>("EmergencyContactUserProfileUserId")
                                        .HasColumnType("varchar(255)");

                                    b2.Property<string>("CountryCode")
                                        .IsRequired()
                                        .HasMaxLength(5)
                                        .HasColumnType("VARCHAR");

                                    b2.Property<string>("CountryPrefix")
                                        .IsRequired()
                                        .HasMaxLength(5)
                                        .HasColumnType("VARCHAR");

                                    b2.Property<string>("Number")
                                        .IsRequired()
                                        .HasMaxLength(15)
                                        .HasColumnType("VARCHAR");

                                    b2.HasKey("EmergencyContactUserProfileUserId");

                                    b2.ToTable("Profile");

                                    b2.WithOwner()
                                        .HasForeignKey("EmergencyContactUserProfileUserId");
                                });

                            b1.OwnsOne("IdentityProvider.ValueObject.PersonName", "Name", b2 =>
                                {
                                    b2.Property<string>("EmergencyContactUserProfileUserId")
                                        .HasColumnType("varchar(255)");

                                    b2.Property<string>("FirstName")
                                        .IsRequired()
                                        .HasMaxLength(25)
                                        .HasColumnType("VARCHAR");

                                    b2.Property<string>("LastName")
                                        .IsRequired()
                                        .HasMaxLength(25)
                                        .HasColumnType("VARCHAR");

                                    b2.Property<string>("NormalizeName")
                                        .IsRequired()
                                        .HasMaxLength(50)
                                        .HasColumnType("VARCHAR");

                                    b2.HasKey("EmergencyContactUserProfileUserId");

                                    b2.ToTable("Profile");

                                    b2.WithOwner()
                                        .HasForeignKey("EmergencyContactUserProfileUserId");
                                });

                            b1.Navigation("Name")
                                .IsRequired();

                            b1.Navigation("Phone")
                                .IsRequired();
                        });

                    b.OwnsOne("IdentityProvider.ValueObject.MedicalInfo", "Medical", b1 =>
                        {
                            b1.Property<string>("UserProfileUserId")
                                .HasColumnType("varchar(255)");

                            b1.Property<string>("Aptitude")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("VARCHAR");

                            b1.Property<string>("Disabilities")
                                .IsRequired()
                                .HasMaxLength(100)
                                .HasColumnType("VARCHAR");

                            b1.HasKey("UserProfileUserId");

                            b1.ToTable("Profile");

                            b1.WithOwner()
                                .HasForeignKey("UserProfileUserId");
                        });

                    b.Navigation("Address");

                    b.Navigation("Bio");

                    b.Navigation("EmergencyContact");

                    b.Navigation("Medical");

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("TimeStamp")
                        .IsRequired();
                });

            modelBuilder.Entity("IdentityProvider.ValueObject.Access", b =>
                {
                    b.HasOne("IdentityProvider.Entity.Permission", null)
                        .WithMany()
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();

                    b.HasOne("IdentityProvider.Entity.ApplicationUser", null)
                        .WithMany("Permissions")
                        .HasForeignKey("ResourceId")
                        .OnDelete(DeleteBehavior.ClientCascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("IdentityProvider.Entity.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("IdentityProvider.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("IdentityProvider.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("IdentityProvider.Entity.ApplicationRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("IdentityProvider.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("IdentityProvider.Entity.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization", b =>
                {
                    b.HasOne("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication", "Application")
                        .WithMany("Authorizations")
                        .HasForeignKey("ApplicationId");

                    b.Navigation("Application");
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreToken", b =>
                {
                    b.HasOne("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication", "Application")
                        .WithMany("Tokens")
                        .HasForeignKey("ApplicationId");

                    b.HasOne("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization", "Authorization")
                        .WithMany("Tokens")
                        .HasForeignKey("AuthorizationId");

                    b.Navigation("Application");

                    b.Navigation("Authorization");
                });

            modelBuilder.Entity("IdentityProvider.Entity.ApplicationUser", b =>
                {
                    b.Navigation("Permissions");

                    b.Navigation("Profile");
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreApplication", b =>
                {
                    b.Navigation("Authorizations");

                    b.Navigation("Tokens");
                });

            modelBuilder.Entity("OpenIddict.EntityFrameworkCore.Models.OpenIddictEntityFrameworkCoreAuthorization", b =>
                {
                    b.Navigation("Tokens");
                });
#pragma warning restore 612, 618
        }
    }
}
