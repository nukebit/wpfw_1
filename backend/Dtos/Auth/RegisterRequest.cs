using System.ComponentModel.DataAnnotations;

namespace PortfolioApi.Dtos.Auth;

public sealed class RegisterRequest
{
    [Required, EmailAddress, StringLength(256)]
    public string Email { get; init; } = string.Empty;

    [Required, StringLength(128)]
    public string Password { get; init; } = string.Empty;
}
