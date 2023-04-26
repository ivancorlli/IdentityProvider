using IdentityProvider.Entity;
using IdentityProvider.Enumerables;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace IdentityProvider.Context
{
	public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
	{

		public DbSet<Permission> Permission {get;private set;} = default!;

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
			builder.Entity<ApplicationUser>().Property(x=>x.Status)
				.HasConversion(x => x.ToString(), x => (UserStatus)Enum.Parse(typeof(UserStatus), x));
			builder.Entity<ApplicationUser>().Property(x=>x.IsAuthenticatedExternaly).HasColumnType("TINYINT");
			builder.Entity<ApplicationUser>().HasMany(x=>x.Permissions).WithOne().HasForeignKey(x=>x.ResourceId);
			// Role
			builder.Entity<ApplicationRole>().Property(x=>x.Type)
				.HasConversion(x => x.ToString(), x => (RoleTypes)Enum.Parse(typeof(RoleTypes), x));
		}
	}
}