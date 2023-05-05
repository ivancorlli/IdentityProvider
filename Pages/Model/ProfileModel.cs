using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ProfileModel
{
    [Display(Name = "Número de teléfono")]
    [Phone(ErrorMessage = "Número de teléfono invalido.")]
    [Required(ErrorMessage = "Número de teléfono requerido.")]
    [StringLength(20,ErrorMessage ="El número de teléfono debe tener entre 5 y 20 digitos.",MinimumLength =5)]
    public string PhoneNumber { get; set; } = string.Empty;
    [Display(Name = "Nombre de usuario")]
    [Required(ErrorMessage = "Nombre de usuario requerido.")]
    [StringLength(20,ErrorMessage ="El nombre de usuario debe tener entre 4 y 20 caracteres.",MinimumLength =4)]
    public string UserName { get; set; } = string.Empty;
}