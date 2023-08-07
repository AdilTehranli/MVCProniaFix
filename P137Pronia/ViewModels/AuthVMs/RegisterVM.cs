using System.ComponentModel.DataAnnotations;

namespace P137Pronia.ViewModels.AuthVMs;

public class RegisterVM
{
    [Required]
    public string Name { get; set; }
    [Required]

    public string Surname { get; set; }
    [Required]

    public string Username { get; set; }
    [Required,DataType(DataType.EmailAddress)]

    public string EmailAddress { get; set; }
    [Required,DataType(DataType.Password)]

    public string Password { get; set; }
    [Required, DataType(DataType.Password),Compare(nameof(Password))]

    public string RepeatPassword { get; set; }
}
