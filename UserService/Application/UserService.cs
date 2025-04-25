using System.Diagnostics.CodeAnalysis;
using UserService.Data.Repository;
using UserService.Domain;

namespace UserService.Application;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Task<User> CreateUserAsync(User user)
    {
        return _userRepository.CreateUserAsync(user);
    }

    public Task<User> UpdateUserAsync(User user)
    {
        return _userRepository.UpdateUserAsync(user);
    }

    public Task DeleteUserAsync(Guid userId)
    {
        return _userRepository.DeleteUserAsync(userId);
    }

    public Task<User?> GetUserByIdAsync(Guid userId)
    {
        return _userRepository.GetUserByIdAsync(userId);
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return _userRepository.GetAllUsersAsync();
    }
}