using FitnessManagement.DTOs;
using FitnessManagement.Models;

namespace FitnessManagement.Services;

public interface IUserService
{
    Task<List<User>> GetAllAsync();

    Task<User?> GetByIdAsync(int id);

    Task<User> CreateAsync(CreateUserDto dto);

    Task<User?> UpdateAsync(int id, UpdateUserDto dto);

    Task<bool> DeleteAsync(int id);
}