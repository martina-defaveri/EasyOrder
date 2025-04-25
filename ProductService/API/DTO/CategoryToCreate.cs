using System.ComponentModel.DataAnnotations;

namespace ProductService.API.DTO;

public record CategoryToCreate(
    [Required(ErrorMessage = "Name is required.")]
    [MaxLength(50, ErrorMessage = "Name cannot exceed 50 characters.")]
    string Name
);