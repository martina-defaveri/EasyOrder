using AddressBookService.Domain;

namespace AddressBookService.Application;

public interface IAddressBookService
{
    Task<Address> CreateAddressAsync(Address address);
    Task<Address> UpdateAddressAsync(Address address);
    Task DeleteAddressAsync(Guid addressId);
    Task<Address?> GetAddressByIdAsync(Guid addressId);
    Task<IEnumerable<Address>> GetAllAddressesAsync();
}