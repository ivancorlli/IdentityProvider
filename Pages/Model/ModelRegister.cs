using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelRegister
{

	[Required(ErrorMessage = "Email Requerido")]
	[EmailAddress(ErrorMessage = "Email invalido")]
	public string Email { get; set; } = string.Empty;


	[Required(ErrorMessage = "Contraseña requerida")]
	[StringLength(100, ErrorMessage = "la contrseña debe tener al menos 6 caracters.", MinimumLength = 6)]
	[DataType(DataType.Password, ErrorMessage = "Contraseña invalida")]
	[Display(Name = "Password")]
	public string Password { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[Display(Name = "Confirm password")]
	[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
	public string ConfirmPassword { get; set; } = string.Empty;
}
