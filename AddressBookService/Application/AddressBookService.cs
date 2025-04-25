using AddressBookService.Data.Repository;
using AddressBookService.Domain;

namespace AddressBookService.Application;

public class AddressBookService(IAddressBookRepository addressBookRepository) : IAddressBookService
{
    public Task<Address> CreateAddressAsync(Address address)
    {
        return addressBookRepository.CreateAddressAsync(address);
    }

    public Task<Address> UpdateAddressAsync(Address address)
    {
        return addressBookRepository.UpdateAddressAsync(address);
    }

    public Task DeleteAddressAsync(Guid addressId)
    {
        return addressBookRepository.DeleteAddressAsync(addressId);
    }

    public Task<Address?> GetAddressByIdAsync(Guid addressId)
    {
        return addressBookRepository.GetAddressByIdAsync(addressId);
    }

    public Task<IEnumerable<Address>> GetAllAddressesAsync()
    {
        return addressBookRepository.GetAllAddressesAsync();
    }
}