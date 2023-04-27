using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ProfileModel
{
    [Display(Name = "Número de teléfono")]
    [Phone(ErrorMessage = "Número de teléfono invalido")]
    [Required(ErrorMessage = "Número de teléfono requerido")]
    [MaxLength(20,ErrorMessage ="El número de teléfono no puede tener mas de 20 digitos")]
    public string PhoneNumber { get; set; } = string.Empty;
    [Display(Name = "Nombre de usuario")]
    [Required(ErrorMessage = "Nombre de usuario requerido")]
    [MaxLength(20,ErrorMessage ="El nombre de usuario no puede tener mas de 20 caracteres")]
    public string UserName { get; set; } = string.Empty;
}