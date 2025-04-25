using System.ComponentModel.DataAnnotations;

namespace ProductService.API.DTO
{
    public record ProductToUpdate(
        [Required(ErrorMessage = "Id is required.")]
        Guid Id,
        
        [Required(ErrorMessage = "Name is required.")]
        [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
        string Name,
        
        [Required(ErrorMessage = "Category is required.")]
        Guid CategoryId,
        
        [Required(ErrorMessage = "Description is required.")]
        string Description,
        
        [Required(ErrorMessage = "Price is required.")]
        [Range(0, 1000000, ErrorMessage = "Price must be between 0 and 1,000,000.")]
        decimal Price
    );
}