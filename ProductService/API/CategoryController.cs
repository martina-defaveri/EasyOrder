using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProductService.API.DTO;
using ProductService.Application;
using ProductService.Domain;

namespace ProductService.API;

[ApiController]
[Route("api/[controller]")]
public class CategoryController(ICategoryService categoryService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateCategory(CategoryToCreate categoryToCreate)
    {
        var category = mapper.Map<Category>(categoryToCreate);
        var createdCategory = await categoryService.CreateCategoryAsync(category).ConfigureAwait(false);
        return CreatedAtAction(nameof(GetCategory), new { id = createdCategory.Id }, createdCategory);
    }
    
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllCategories() =>
        Ok(await categoryService.GetAllCategoriesAsync().ConfigureAwait(false));
    
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetCategory([Required]Guid id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id).ConfigureAwait(false);
        return category is not null ? Ok(category) : NotFound();
    }
    
    [HttpPut]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateCategory(CategoryToUpdate categoryToUpdate)
    {
        var category = mapper.Map<Category>(categoryToUpdate);
        var categoryFound = await categoryService.GetCategoryByIdAsync(category.Id).ConfigureAwait(false);
        if (categoryFound is null) return NotFound();
        var categoryModified = await categoryService.UpdateCategoryAsync(category).ConfigureAwait(false);
        return Ok(categoryModified);
    }
    
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> DeleteCategory([Required]Guid id)
    {
        var category = await categoryService.GetCategoryByIdAsync(id).ConfigureAwait(false);
        if (category is null) return  NotFound();
        await categoryService.DeleteCategoryAsync(id).ConfigureAwait(false);
        return NoContent();
    }
}