using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelRecovery
{
	[Required]
	[EmailAddress]
	public string Email { get; set; } = default!;
}
