namespace AddressBookService.Domain;

public record Address(
    Guid Id,
    Guid UserId,
    string Recipient,
    string Street,
    string City,
    string State,
    string ZipCode,
    string Country,
    string Phone);