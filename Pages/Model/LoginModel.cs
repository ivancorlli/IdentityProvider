using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Email requerido")]
		[Display(Name = "Correo electronico o nombre de usuario")]
		public string Email { get; set; } = string.Empty;
		[Required(ErrorMessage = "Contraseña requerida")]
		[DataType(DataType.Password, ErrorMessage = "Formato incorrecto")]
		[Display(Name = "Contraseña")]
		public string Password { get; set; } = string.Empty;

        [Display(Name = "Recordarme")]
        public bool Remember { get; set; } = false;
	}
}
