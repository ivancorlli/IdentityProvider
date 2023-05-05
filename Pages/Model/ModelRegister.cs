using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelRegister
{

	[Required(ErrorMessage = "Campo requerido.")]
	[EmailAddress(ErrorMessage = "Correo electrónico invalido.")]
	[Display(Name = "Correo electrónico")]
	[StringLength(25,ErrorMessage = "El correo electrónico debe tener entre 6 y 25 caracteres.",MinimumLength =6)]
	public string Email { get; set; } = string.Empty;


	[Required(ErrorMessage = "Campo requerido")]
	[StringLength(25, ErrorMessage = "La contrseña debe tener entre 6 y 25 caracteres.", MinimumLength = 6)]
	[DataType(DataType.Password, ErrorMessage = "Contraseña invalida")]
	[Display(Name = "Contraseña")]
	public string Password { get; set; } = string.Empty;

	[DataType(DataType.Password)]
	[Required(ErrorMessage = "Campo requerido")]
	[Display(Name = "Confirmar contraseña")]
	[Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
	public string ConfirmPassword { get; set; } = string.Empty;
}
