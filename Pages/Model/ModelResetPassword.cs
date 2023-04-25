using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace IdentityProvider.Pages.Model;

public class ModelResetPassword
{

	[Required(ErrorMessage ="Contraseña requerida")]
	[Display(Name="Contraseña")]
	[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
	[DataType(DataType.Password)]
	public string Password { get; set; } = default!;

	[DataType(DataType.Password)]
	[Display(Name = "Confirmar contraseña")]
	[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
	[Required(ErrorMessage ="Confirmar contraseña requerida")]
	public string? ConfirmPassword { get; set; }

}
