using System.ComponentModel.DataAnnotations;

namespace FitnessManagement.Contracts.Users;

public sealed class CreateUserRequest
{
    [Required, StringLength(100)]
    public string FullName { get; init; } = string.Empty;

    [Required, EmailAddress, StringLength(255)]
    public string Email { get; init; } = string.Empty;

    [Phone, StringLength(20)]
    public string? PhoneNumber { get; init; }
}
