using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTO;
using ProductService.Application;
using ProductService.Domain;

namespace ProductService.API;

[ApiController]
[Route("api/[controller]")]
public class ProductController(IProductService productService, ICategoryService categoryService, IMapper mapper) : ControllerBase
{
    private readonly IProductService _productService = productService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IMapper _mapper = mapper;

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateProduct(ProductToCreate productToCreate)
    {
        var product = _mapper.Map<Product>(productToCreate);
        var createdProduct = await _productService.CreateProductAsync(product).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetProduct), new { id = createdProduct.Id }, createdProduct);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProducts() =>
        Ok(await _productService.GetAllProductsAsync().ConfigureAwait(false));

    [HttpGet("Category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllProductsByCategory(Guid categoryId)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId).ConfigureAwait(false);
        if (category is null)
        {
            return NotFound("Category not found.");
        }
        return Ok(await _productService.GetProductsByCategoryAsync(categoryId).ConfigureAwait(false));
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProduct([Required]Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id).ConfigureAwait(false);
        return product is not null ? Ok(product) : NotFound();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateProduct(ProductToUpdate productToUpdate)
    {
        var product = _mapper.Map<Product>(productToUpdate);
        var productFound = await _productService.GetProductByIdAsync(product.Id).ConfigureAwait(false);
        if (productFound is null) return NotFound();
        var productModified = await _productService.UpdateProductAsync(product).ConfigureAwait(false);
        return Ok(productModified);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteProduct([Required]Guid id)
    {
        var product = await _productService.GetProductByIdAsync(id).ConfigureAwait(false);
        if (product is null) return  NotFound();
        await _productService.DeleteProductAsync(id).ConfigureAwait(false);
        return NoContent();
    }
}