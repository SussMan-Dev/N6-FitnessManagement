using FitnessManagement.Contracts.Users;
using FitnessManagement.Data;
using FitnessManagement.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;

namespace FitnessManagement.Controllers;

[ApiController]
[Route("user")]
public sealed class UsersController(AppDbContext dbContext) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType<IReadOnlyList<UserResponse>>(StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await dbContext.Users
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .Select(x => ToResponse(x))
            .ToListAsync(cancellationToken);

        return Ok(users);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponse>> GetById(int id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        return user is null ? NotFound() : Ok(ToResponse(user));
    }

    [HttpPost]
    [ProducesResponseType<UserResponse>(StatusCodes.Status201Created)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Create(
        CreateUserRequest request,
        CancellationToken cancellationToken)
    {
        var email = NormalizeEmail(request.Email);
        if (await dbContext.Users.AnyAsync(x => x.Email == email, cancellationToken))
        {
            return EmailConflict(email);
        }

        var now = DateTime.UtcNow;
        var user = new User
        {
            FullName = request.FullName.Trim(),
            Email = email,
            PhoneNumber = NormalizePhoneNumber(request.PhoneNumber),
            CreatedAtUtc = now,
            UpdatedAtUtc = now
        };

        dbContext.Users.Add(user);

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateEntry(exception))
        {
            return EmailConflict(email);
        }

        var response = ToResponse(user);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType<UserResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType<ValidationProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponse>> Update(
        int id,
        UpdateUserRequest request,
        CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var email = NormalizeEmail(request.Email);
        if (await dbContext.Users.AnyAsync(x => x.Id != id && x.Email == email, cancellationToken))
        {
            return EmailConflict(email);
        }

        user.FullName = request.FullName.Trim();
        user.Email = email;
        user.PhoneNumber = NormalizePhoneNumber(request.PhoneNumber);
        user.UpdatedAtUtc = DateTime.UtcNow;

        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException exception) when (IsDuplicateEntry(exception))
        {
            return EmailConflict(email);
        }

        return Ok(ToResponse(user));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var user = await dbContext.Users.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        dbContext.Users.Remove(user);
        await dbContext.SaveChangesAsync(cancellationToken);

        return NoContent();
    }

    private ConflictObjectResult EmailConflict(string email) => Conflict(new ProblemDetails
    {
        Status = StatusCodes.Status409Conflict,
        Title = "Email already exists",
        Detail = $"A user with email '{email}' already exists."
    });

    private static UserResponse ToResponse(User user) => new(
        user.Id,
        user.FullName,
        user.Email,
        user.PhoneNumber,
        user.CreatedAtUtc,
        user.UpdatedAtUtc);

    private static string NormalizeEmail(string email) => email.Trim().ToLowerInvariant();

    private static string? NormalizePhoneNumber(string? phoneNumber) =>
        string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();

    private static bool IsDuplicateEntry(DbUpdateException exception) =>
        exception.InnerException is MySqlException { Number: 1062 };
}
