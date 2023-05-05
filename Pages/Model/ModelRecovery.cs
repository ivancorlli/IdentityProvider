using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ModelRecovery
{
    [EmailAddress(ErrorMessage = "Correo electrónico invalido")]
    [Required(ErrorMessage = "Correo electrónico requerido")]
    [Display(Name = "Correo electrónico")]
    [DataType(DataType.PhoneNumber,ErrorMessage = "Formato invalido")]
    [StringLength(25,ErrorMessage ="El correo electronico debe tener etre 6 y 25 caracteres.",MinimumLength =6)]
    public string Email { get; set; } = default!;
}
