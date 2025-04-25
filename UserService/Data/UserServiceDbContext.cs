using Microsoft.EntityFrameworkCore;
using UserService.Domain;

namespace UserService.Data;

public class UserServiceDbContext(DbContextOptions<UserServiceDbContext> options) : DbContext(options)
{
    public virtual DbSet<User> Users => Set<User>();
}