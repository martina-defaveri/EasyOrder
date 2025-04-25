using AddressBookService.API.DTO;
using AddressBookService.Domain;
using AutoMapper;

namespace AddressBookService.API.Mapping;

public class AddressMapping: Profile
{
    public AddressMapping()
    {
        CreateMap<Address, AddressToCreate>().ConstructUsing(
            src => new AddressToCreate(src.UserId, src.Recipient, src.Street, src.City, src.State, src.ZipCode, src.Country, src.Phone));
        CreateMap<Address, AddressToUpdate>(). ConstructUsing(
            src => new AddressToUpdate(src.Id, src.UserId, src.Recipient, src.Street, src.City, src.State, src.ZipCode, src.Country, src.Phone));
        CreateMap<AddressToUpdate, Address>().ConstructUsing(
            src => new Address(src.Id, src.UserId, src.Recipient, src.Street, src.City, src.State, src.ZipCode, src.Country, src.Phone));
        CreateMap<AddressToCreate, Address>().ConstructUsing(
            src => new Address(Guid.Empty, src.UserId, src.Recipient, src.Street, src.City, src.State, src.ZipCode, src.Country, src.Phone));
    }
}