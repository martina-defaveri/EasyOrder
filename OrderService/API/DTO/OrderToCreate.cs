using System.ComponentModel.DataAnnotations;

namespace OrderService.API.DTO;

public record OrderToCreate(
    [Required(ErrorMessage = "User GUID is required.")]
    Guid UserId,
    
    [Required(ErrorMessage = "Address GUID is required.")]
    Guid AddressId,
    
    [Required(ErrorMessage = "Order products are required.")]
    [MinLength(1, ErrorMessage = "You must insert at least one product.")]
    ICollection<OrderProductToCreate> OrderProducts
);