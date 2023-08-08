using System.ComponentModel.DataAnnotations;

namespace P137Pronia.ViewModels.AuthVMs;

public class LoginVM
{
    [Required]
    public string EmailOrUsername { get; set; }
    [Required,DataType(DataType.Password),MinLength(8)]

    public string Password { get; set; }
    public bool RemmemberMe { get; set; }
}
