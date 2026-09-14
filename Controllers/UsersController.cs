using FitnessManagement.DTOs;
using FitnessManagement.Models;
using FitnessManagement.Services;
using Microsoft.AspNetCore.Mvc;

namespace FitnessManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: api/users
    [HttpGet]
    public async Task<ActionResult<List<User>>> GetAll()
    {
        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    // GET: api/users/1
    [HttpGet("{id}")]
    public async Task<ActionResult<User>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found"
            });
        }

        return Ok(user);
    }

    // POST: api/users
    [HttpPost]
    public async Task<ActionResult<User>> Create(CreateUserDto dto)
    {
        var user = await _userService.CreateAsync(dto);

        return CreatedAtAction(
            nameof(GetById),
            new { id = user.Id },
            user
        );
    }

    // PUT: api/users/1
    [HttpPut("{id}")]
    public async Task<ActionResult<User>> Update(
        int id,
        UpdateUserDto dto)
    {
        var user = await _userService.UpdateAsync(id, dto);

        if (user == null)
        {
            return NotFound(new
            {
                message = "User not found"
            });
        }

        return Ok(user);
    }

    // DELETE: api/users/1
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var deleted = await _userService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = "User not found"
            });
        }

        return NoContent();
    }
}