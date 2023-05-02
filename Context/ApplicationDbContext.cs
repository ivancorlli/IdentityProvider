using IdentityProvider.Entity;
using IdentityProvider.Enumerables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Context
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        public DbSet<Permission> Permission { get; private set; } = default!;
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfiguration(new UserProfileConfiguration());
            builder.ApplyConfiguration(new PermissionConfiguration());
            builder.ApplyConfiguration(new AccessConfiguration());
            // User
            builder.Entity<ApplicationUser>().Property(x => x.Status).HasConversion(x => x.ToString(), x => (UserStatus)Enum.Parse(typeof(UserStatus), x));
            builder.Entity<ApplicationUser>().Property(x => x.IsAuthenticatedExternaly).IsRequired();
            builder.Entity<ApplicationUser>().Property(x => x.PhoneTwoFactorEnabled).IsRequired();
            builder.Entity<ApplicationUser>().HasMany(x => x.Permissions).WithOne().HasForeignKey(x => x.ResourceId).OnDelete(DeleteBehavior.ClientCascade);
            builder.Entity<ApplicationUser>().HasOne(x=>x.Profile).WithOne().HasForeignKey<UserProfile>(x=>x.UserId).OnDelete(DeleteBehavior.ClientCascade);
            // Role
            builder.Entity<ApplicationRole>().Property(x => x.Type).HasColumnType("VARCHAR").HasMaxLength(25);
        }
    }
}