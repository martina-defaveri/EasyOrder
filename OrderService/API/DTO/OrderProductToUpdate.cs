using System.ComponentModel.DataAnnotations;

namespace OrderService.API.DTO;

public record OrderProductToUpdate(
    [Required(ErrorMessage = "Order ID is required.")]
    Guid OrderId,
    
    [Required(ErrorMessage = "Product ID is required.")]
    Guid ProductId,
    
    [Required(ErrorMessage = "Number of products is required.")]
    [Range(1, 1000000, ErrorMessage = "You must insert a minimum of one product to 1,000,000.")]
    int NumberOfProducts,
    
    [Required(ErrorMessage = "Price is required.")]
    [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000.")]
    decimal ProductPrice);