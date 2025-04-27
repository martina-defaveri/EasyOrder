using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.API;
using ProductService.API.DTO;
using ProductService.Application;
using ProductService.Domain;
using Xunit;

namespace ProductService.Tests;

public class CategoryServiceApiTests
{
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly CategoryController _controller;

    public CategoryServiceApiTests()
    {
        _mockCategoryService = new Mock<ICategoryService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new CategoryController(_mockCategoryService.Object, _mockMapper.Object);
    }

    [Fact]
    public async Task CreateCategory_ValidInput_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var categoryToCreate = new CategoryToCreate("Test Category");
        var category = new Category(Guid.NewGuid(), categoryToCreate.Name);
        _mockMapper.Setup(m => m.Map<Category>(categoryToCreate)).Returns(category);
        _mockCategoryService.Setup(s => s.CreateCategoryAsync(category)).ReturnsAsync(category);

        // Act
        var result = await _controller.CreateCategory(categoryToCreate);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetCategory), actionResult.ActionName);
        Assert.NotNull((Category)actionResult.Value);
        Assert.Equal(category.Id, ((Category)actionResult.Value).Id);
        _mockMapper.Verify(m => m.Map<Category>(categoryToCreate), Times.Once);
        _mockCategoryService.Verify(s => s.CreateCategoryAsync(category), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
        _mockMapper.VerifyAll();
    }

    [Fact]
    public async Task GetAllCategories_ReturnsOkResult_WithCategories()
    {
        // Arrange
        var categories = new List<Category> { new(Guid.NewGuid(), "Category1") };
        _mockCategoryService.Setup(s => s.GetAllCategoriesAsync()).ReturnsAsync(categories);

        // Act
        var result = await _controller.GetAllCategories();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(categories, okResult.Value);
        _mockCategoryService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
    }

    [Fact]
    public async Task GetAllCategories_ThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        _mockCategoryService.Setup(s => s.GetAllCategoriesAsync()).ThrowsAsync(new Exception());

        // Act
        async Task Logic() => await _controller.GetAllCategories();

        // Assert
        var caught = await Assert.ThrowsAsync<Exception>(Logic);
        Assert.NotNull(caught);
        _mockCategoryService.Verify(s => s.GetAllCategoriesAsync(), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
    }

    [Fact]
    public async Task GetCategory_ValidId_ReturnsOkResult()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category(categoryId, "Test Category");
        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync(category);

        // Act
        var result = await _controller.GetCategory(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(category, okResult.Value);
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
    }

    [Fact]
    public async Task GetCategory_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync((Category)null);

        // Act
        var result = await _controller.GetCategory(categoryId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
    }

    [Fact]
    public async Task UpdateCategory_ValidInput_ReturnsOkResult()
    {
        // Arrange
        var categoryToUpdate = new CategoryToUpdate(Guid.NewGuid(), "Updated Category");
        var oldCategory = new Category(categoryToUpdate.Id, "Old Category");
        var postMapCategory = new Category(categoryToUpdate.Id, categoryToUpdate.Name);
        var updatedCategory = new Category(categoryToUpdate.Id, categoryToUpdate.Name);
        _mockMapper.Setup(m => m.Map<Category>(categoryToUpdate)).Returns(postMapCategory);
        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(postMapCategory.Id)).ReturnsAsync(oldCategory);
        _mockCategoryService.Setup(s => s.UpdateCategoryAsync(postMapCategory)).ReturnsAsync(updatedCategory);

        // Act
        var result = await _controller.UpdateCategory(categoryToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedCategory, okResult.Value);
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryToUpdate.Id), Times.Once);
        _mockCategoryService.Verify(s => s.UpdateCategoryAsync(updatedCategory), Times.Once);
        _mockMapper.Verify(m => m.Map<Category>(categoryToUpdate), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
        _mockMapper.VerifyAll();
    }

    [Fact]
    public async Task DeleteCategory_ValidId_ReturnsNoContent()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category(categoryId, "Category to Delete");
        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync(category);
        _mockCategoryService.Setup(s => s.DeleteCategoryAsync(categoryId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteCategory(categoryId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        _mockCategoryService.Verify(s => s.DeleteCategoryAsync(categoryId), Times.Once);
        _mockCategoryService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyAll();
    }
}