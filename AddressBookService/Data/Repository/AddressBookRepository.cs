using System.Collections.Immutable;
using Microsoft.EntityFrameworkCore;
using AddressBookService.Domain;

namespace AddressBookService.Data.Repository;

public class AddressBookRepository(AddressBookServiceDbContext dbContext) : IAddressBookRepository
{
    public async Task<Address> CreateAddressAsync(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        var insertedAddressBook = dbContext.Addresses.Add(address);
        await dbContext.SaveChangesAsync();
        return insertedAddressBook.Entity;
    }

    public async Task<Address> UpdateAddressAsync(Address address)
    {
        ArgumentNullException.ThrowIfNull(address);
        var foundAddress = await dbContext.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == address.Id);
        ArgumentNullException.ThrowIfNull(foundAddress);

        var updatedAddress = dbContext.Addresses.Update(address);
        await dbContext.SaveChangesAsync();
        return updatedAddress.Entity;
    }

    public async Task<bool> DeleteAddressAsync(Guid addressId)
    {
        var foundAddress = await dbContext.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == addressId);
        ArgumentNullException.ThrowIfNull(foundAddress);
        dbContext.Addresses.Remove(foundAddress);
        return await dbContext.SaveChangesAsync() > 0;
    }

    public async Task<Address?> GetAddressByIdAsync(Guid addressId)
    {
        var foundAddress = await dbContext.Addresses.AsNoTracking().FirstOrDefaultAsync(x => x.Id == addressId);
        return foundAddress;
    }

    public Task<IEnumerable<Address>> GetAllAddressesAsync()
    {
        var foundAddress = dbContext.Addresses.AsNoTracking().ToImmutableList();
        return Task.FromResult<IEnumerable<Address>>(foundAddress);
    }
}