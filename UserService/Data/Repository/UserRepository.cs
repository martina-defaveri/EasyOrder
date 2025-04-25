using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using UserService.Domain;

namespace UserService.Data.Repository;

public class UserRepository(UserServiceDbContext dbContext) : IUserRepository
{
    public async Task<User> CreateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var insertedUser = dbContext.Users.Add(user);
        await dbContext.SaveChangesAsync();
        return insertedUser.Entity;
    }

    public async Task<User> UpdateUserAsync(User user)
    {
        ArgumentNullException.ThrowIfNull(user);
        var foundUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == user.Id);
        ArgumentNullException.ThrowIfNull(foundUser);

        var updatedUser = dbContext.Users.Update(user);
        await dbContext.SaveChangesAsync();
        return updatedUser.Entity;
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var foundUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
        ArgumentNullException.ThrowIfNull(foundUser);
        dbContext.Users.Remove(foundUser);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        var foundUser = await dbContext.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Id == userId);
        return foundUser;
    }

    public Task<IEnumerable<User>> GetAllUsersAsync()
    {
        var foundUsers = dbContext.Users.AsNoTracking().ToImmutableList();
        return Task.FromResult<IEnumerable<User>>(foundUsers);
    }
}