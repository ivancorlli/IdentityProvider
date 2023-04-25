using System.ComponentModel.DataAnnotations;

namespace IdentityProvider.Pages.Model;

public class ConfigPhoneModel
{
    [Display(Name = "Número de teléfono")]
    [Phone(ErrorMessage ="Número de teléfono invalido")]
    [Required(ErrorMessage ="Número de teléfono requerido")]
    public string PhoneNumber {get;set;} = string.Empty;    
}