using FitnessManagement.Data;
using FitnessManagement.DTOs;
using FitnessManagement.Models;
using Microsoft.EntityFrameworkCore;

namespace FitnessManagement.Services;

public class UserService : IUserService
{
    private readonly FitnessDbContext _context;

    public UserService(FitnessDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllAsync()
    {
        return await _context.Users
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<User?> GetByIdAsync(int id)
    {
        return await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<User> CreateAsync(CreateUserDto dto)
    {
        var user = new User
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Phone = dto.Phone,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            Address = dto.Address,
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<User?> UpdateAsync(int id, UpdateUserDto dto)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return null;
        }

        user.FullName = dto.FullName;
        user.Email = dto.Email;
        user.Phone = dto.Phone;
        user.DateOfBirth = dto.DateOfBirth;
        user.Gender = dto.Gender;
        user.Address = dto.Address;

        await _context.SaveChangesAsync();

        return user;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var user = await _context.Users.FindAsync(id);

        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);

        await _context.SaveChangesAsync();

        return true;
    }
}