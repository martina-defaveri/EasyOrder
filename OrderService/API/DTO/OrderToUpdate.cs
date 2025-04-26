using System.ComponentModel.DataAnnotations;

namespace OrderService.API.DTO
{
    public record OrderToUpdate(
        [Required(ErrorMessage = "Order GUID is required.")]
        Guid Id,
        
        [Required(ErrorMessage = "User GUID is required.")]
        Guid UserId,
    
        [Required(ErrorMessage = "Address GUID is required.")]
        Guid AddressId,
    
        [Required(ErrorMessage = "Order products are required.")]
        [MinLength(1, ErrorMessage = "You must insert at least one product.")]
        ICollection<OrderProductToUpdate> OrderProducts
    );
}