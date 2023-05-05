using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelResetPassword
{

	[Required(ErrorMessage ="Campo requerido.")]
	[Display(Name="Contraseña")]
	[StringLength(25, ErrorMessage = "La contraseña debe tener entre 6 y 25 caracteres.", MinimumLength = 6)]
	[DataType(DataType.Password)]
	public string Password { get; set; } = default!;

	[DataType(DataType.Password)]
	[Display(Name = "Confirmar contraseña")]
	[Compare("Password", ErrorMessage = "Las contraseñas no coinciden.")]
	[Required(ErrorMessage ="Campo requerido.")]
	public string ConfirmPassword { get; set; } =default!;

}
