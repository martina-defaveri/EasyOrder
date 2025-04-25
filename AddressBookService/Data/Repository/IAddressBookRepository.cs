using AddressBookService.Domain;

namespace AddressBookService.Data.Repository;

public interface IAddressBookRepository
{
    Task<Address> CreateAddressAsync(Address address);
    Task<Address> UpdateAddressAsync(Address address);
    Task<bool> DeleteAddressAsync(Guid addressId);
    Task<Address?> GetAddressByIdAsync(Guid addressId);
    Task<IEnumerable<Address>> GetAllAddressesAsync();
}