using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using ProductService.API;
using ProductService.API.DTO;
using ProductService.Application;
using ProductService.Domain;
using Xunit;

namespace ProductService.Tests;

public class ProductControllerTests
{
    private readonly Mock<IProductService> _mockProductService;
    private readonly Mock<ICategoryService> _mockCategoryService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProductController _controller;

    public ProductControllerTests()
    {
        _mockProductService = new Mock<IProductService>();
        _mockCategoryService = new Mock<ICategoryService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new ProductController(_mockProductService.Object, _mockCategoryService.Object, _mockMapper.Object);
    }
    
    [Fact]
    public async Task CreateProduct_ValidInput_ReturnsCreatedAtActionResult()
    {
        // Arrange
        var categoryGuid = Guid.NewGuid();
        var productToCreate = new ProductToCreate("Test Product", categoryGuid, "Description", 100.0m);
        var productMap = new Product(Guid.Empty, productToCreate.Name, categoryGuid, productToCreate.Description, productToCreate.Price);
        var product = new Product(Guid.NewGuid(), productToCreate.Name, categoryGuid, productToCreate.Description, productToCreate.Price);

        _mockMapper.Setup(m => m.Map<Product>(productToCreate)).Returns(productMap);
        _mockProductService.Setup(s => s.CreateProductAsync(productMap)).ReturnsAsync(product);

        // Act
        var result = await _controller.CreateProduct(productToCreate);

        // Assert
        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetProduct), actionResult.ActionName);
        Assert.Equal(product.Id, ((Product)actionResult.Value).Id);

        // Verify
        _mockMapper.Verify(m => m.Map<Product>(productToCreate), Times.Once);
        _mockProductService.Verify(s => s.CreateProductAsync(productMap), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
        _mockMapper.VerifyAll();
    }
    
    [Fact]
    public async Task GetAllProducts_ReturnsOk_WithProducts()
    {
        // Arrange
        var products = new List<Product>
        {
            new(Guid.NewGuid(), "Product 1", Guid.NewGuid(), "Description 1", 50.0m),
            new(Guid.NewGuid(), "Product 2", Guid.NewGuid(), "Description 2", 100.0m)
        };
        _mockProductService.Setup(s => s.GetAllProductsAsync()).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllProducts();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(products, okResult.Value);

        // Verify
        _mockProductService.Verify(s => s.GetAllProductsAsync(), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
    }
    
    [Fact]
    public async Task GetAllProductsByCategory_ValidCategoryId_ReturnsOk()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new Category(categoryId, "Test Category");
        var products = new List<Product>
        {
            new(Guid.NewGuid(), "Product 1", category.Id, "Description 1", 50.0m)
        };

        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync(category);
        _mockProductService.Setup(s => s.GetProductsByCategoryAsync(categoryId)).ReturnsAsync(products);

        // Act
        var result = await _controller.GetAllProductsByCategory(categoryId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(products, okResult.Value);

        // Verify
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        _mockProductService.Verify(s => s.GetProductsByCategoryAsync(categoryId), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockCategoryService.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
        _mockCategoryService.VerifyAll();
    }

    [Fact]
    public async Task GetAllProductsByCategory_CategoryNotFound_ReturnsNotFound()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        _mockCategoryService.Setup(s => s.GetCategoryByIdAsync(categoryId)).ReturnsAsync((Category)null);

        // Act
        var result = await _controller.GetAllProductsByCategory(categoryId);

        // Assert
        var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
        Assert.Equal("Category not found.", notFoundResult.Value);

        // Verify
        _mockCategoryService.Verify(s => s.GetCategoryByIdAsync(categoryId), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
        _mockCategoryService.VerifyAll();
        _mockCategoryService.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetProduct_ValidId_ReturnsOk()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Product 1", Guid.NewGuid(), "Description 1", 50.0m);
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(product);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(product, okResult.Value);

        // Verify
        _mockProductService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
    }

    [Fact]
    public async Task GetProduct_InvalidId_ReturnsNotFound()
    {
        // Arrange
        var productId = Guid.NewGuid();
        _mockProductService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync((Product)null);

        // Act
        var result = await _controller.GetProduct(productId);

        // Assert
        Assert.IsType<NotFoundResult>(result);

        // Verify
        _mockProductService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
    }
    
    [Fact]
    public async Task UpdateProduct_ValidInput_ReturnsOk()
    {
        // Arrange
        var categoryGuid = Guid.NewGuid();
        var productToUpdate = new ProductToUpdate(Guid.NewGuid(), "Updated Product", categoryGuid, "Updated Description", 200.0m);
        var mapProduct = new Product(productToUpdate.Id, productToUpdate.Name, categoryGuid, productToUpdate.Description, productToUpdate.Price);
        var oldProduct = new Product(productToUpdate.Id, "Old name", Guid.NewGuid(), "old description", productToUpdate.Price);
        var updatedProduct = new Product(productToUpdate.Id, productToUpdate.Name, categoryGuid, productToUpdate.Description, productToUpdate.Price);

        _mockMapper.Setup(m => m.Map<Product>(productToUpdate)).Returns(mapProduct);
        _mockProductService.Setup(s => s.GetProductByIdAsync(mapProduct.Id)).ReturnsAsync(oldProduct);
        _mockProductService.Setup(s => s.UpdateProductAsync(mapProduct)).ReturnsAsync(updatedProduct);

        // Act
        var result = await _controller.UpdateProduct(productToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(updatedProduct, okResult.Value);

        // Verify
        _mockProductService.Verify(s => s.GetProductByIdAsync(productToUpdate.Id), Times.Once);
        _mockProductService.Verify(s => s.UpdateProductAsync(mapProduct), Times.Once);
        _mockMapper.Verify(m => m.Map<Product>(productToUpdate), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockMapper.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
        _mockMapper.VerifyAll();
    }
    
    [Fact]
    public async Task DeleteProduct_ValidId_ReturnsNoContent()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new Product(productId, "Product to Delete", Guid.NewGuid(), "Description", 50.0m);

        _mockProductService.Setup(s => s.GetProductByIdAsync(productId)).ReturnsAsync(product);
        _mockProductService.Setup(s => s.DeleteProductAsync(productId)).ReturnsAsync(true);

        // Act
        var result = await _controller.DeleteProduct(productId);

        // Assert
        Assert.IsType<NoContentResult>(result);

        // Verify
        _mockProductService.Verify(s => s.GetProductByIdAsync(productId), Times.Once);
        _mockProductService.Verify(s => s.DeleteProductAsync(productId), Times.Once);
        _mockProductService.VerifyNoOtherCalls();
        _mockProductService.VerifyAll();
    }
}