using AutoMapper;
using UserService.API.DTO;
using UserService.Domain;

namespace UserService.API.Mapping;

public class UserMapping: Profile
{
    public UserMapping()
    {
        CreateMap<User, UserToCreate>().ConstructUsing(
            src => new UserToCreate(src.FirstName, src.LastName, src.Email, src.Password));
        CreateMap<User, UserToUpdate>(). ConstructUsing(
            src => new UserToUpdate(src.Id, src.FirstName, src.LastName, src.Email, src.Password));
        CreateMap<UserToUpdate, User>().ConstructUsing(
            src => new User(src.Id, src.FirstName, src.LastName, src.Email, src.Password));
        CreateMap<UserToCreate, User>().ConstructUsing(
            src => new User(Guid.Empty, src.FirstName, src.LastName, src.Email, src.Password));
    }
}