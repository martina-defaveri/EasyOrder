using System.ComponentModel.DataAnnotations;

namespace AddressBookService.API.DTO
{
    public record AddressToUpdate(
        [Required(ErrorMessage = "Id is required.")]
        Guid Id,
        
        [Required(ErrorMessage = "User GUID is required.")]
        Guid UserId,
    
        [Required(ErrorMessage = "Recipient is required.")]
        string Recipient,
    
        [Required(ErrorMessage = "Street is required.")]
        string Street,
    
        [Required(ErrorMessage = "City is required.")]
        string City,
    
        [Required(ErrorMessage = "State is required.")]
        string State,
    
        [Required(ErrorMessage = "Zip code is required.")]
        string ZipCode,
    
        [Required(ErrorMessage = "Country is required.")]
        string Country,
    
        [Required(ErrorMessage = "Phone is required.")]
        string Phone
    );
}