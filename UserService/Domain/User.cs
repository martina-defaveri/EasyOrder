namespace UserService.Domain;

public record User (Guid Id, string FirstName, string LastName, string Email, string Password);