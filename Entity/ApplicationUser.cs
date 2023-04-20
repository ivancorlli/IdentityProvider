using Microsoft.AspNetCore.Identity;

namespace IdentityProvider.Entity
{
	public class ApplicationUser : IdentityUser
	{
		public ApplicationUser()
		{
		}

		public ApplicationUser(string userName) : base(userName)
		{
		}
	}
}
