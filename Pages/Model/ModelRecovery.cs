using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelRecovery
{
    [EmailAddress(ErrorMessage = "Correo electrónico invalido")]
    [Required(ErrorMessage = "Correo electrónico requerido")]
    [Display(Name = "Correo electrónico")]
    [DataType(DataType.PhoneNumber,ErrorMessage = "Formato invalido")]
    public string Email { get; set; } = default!;
}
