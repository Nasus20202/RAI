using System.ComponentModel.DataAnnotations;

namespace Lab2.Web.Models.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Username is required")]
    public string? Username { get; set; }

    [Required(ErrorMessage = "Password is required")]
    [DataType(DataType.Password)]
    public string? Password { get; set; }

    public string? AuthenticationMessage { get; set; }

    public LoginViewModel() { }

    public LoginViewModel(string? authenticationMessage)
    {
        AuthenticationMessage = authenticationMessage;
    }
}
