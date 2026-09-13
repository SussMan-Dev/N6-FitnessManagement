namespace FitnessManagement.Contracts.Users;

public sealed record UserResponse(
    int Id,
    string FullName,
    string Email,
    string? PhoneNumber,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc);
