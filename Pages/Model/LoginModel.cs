using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model
{
	public class LoginModel
	{
		[Required(ErrorMessage = "Campo requerido.")]
		[Display(Name = "Correo electrónico o nombre de usuario")]
		[StringLength(25,ErrorMessage = "El dato ingresado no debe tener más de 25 caracteres.")]
		public string Email { get; set; } = string.Empty;
		[StringLength(25,ErrorMessage = "El dato ingresado no debe tener más de 25 caracteres.")]
		[Required(ErrorMessage = "Campo requerido.")]
		[DataType(DataType.Password, ErrorMessage = "Formato incorrecto.")]
		[Display(Name = "Contraseña")]
		public string Password { get; set; } = string.Empty;
        [Display(Name = "Recordarme")]
        public bool Remember { get; set; } = false;
	}
}
