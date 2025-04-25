using AddressBookService.Domain;
using Microsoft.EntityFrameworkCore;

namespace AddressBookService.Data;

public class AddressBookServiceDbContext(DbContextOptions<AddressBookServiceDbContext> options) : DbContext(options)
{
    public virtual DbSet<Address> Addresses => Set<Address>();
}