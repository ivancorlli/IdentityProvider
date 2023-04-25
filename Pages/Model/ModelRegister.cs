using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelRegister
{

	[Required(ErrorMessage = "Correo electrónico Requerido")]
	[EmailAddress(ErrorMessage = "Correo electrónico invalido")]
	[Display(Name = "Correo electrónico")]
	public string Email { get; set; } = string.Empty;


	[Required(ErrorMessage = "Contraseña requerida")]
	[StringLength(25, ErrorMessage = "La contrseña debe tener al menos 6 caracters.", MinimumLength = 6)]
	[DataType(DataType.Password, ErrorMessage = "Contraseña invalida")]
	[Display(Name = "Contraseña")]
	public string Password { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Confirmar Contraseña requerido")]
	[Display(Name = "Confirmar Contraseña")]
	[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
	public string ConfirmPassword { get; set; } = string.Empty;
}
