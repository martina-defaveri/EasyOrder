using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using OrderService.API;
using OrderService.API.DTO;
using OrderService.Application;
using OrderService.Data.Repository;
using OrderService.Domain;
using Xunit;

namespace OrderService.Tests;

public class OrderServiceApiTests
{
    private readonly Mock<IOrderService> _mockOrderService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly OrderController _controller;
    private readonly Mock<IOrderRepository> _mockRepository;

    public OrderServiceApiTests()
    {
        _mockOrderService = new Mock<IOrderService>();
        _mockMapper = new Mock<IMapper>();
        _controller = new OrderController(_mockOrderService.Object, _mockMapper.Object);
        _mockRepository = new Mock<IOrderRepository>();
    }

    [Fact]
    public async Task CreateOrder_ReturnsCreatedAtAction_WithValidData()
    {
        // Arrange
        var orderToCreate = new OrderToCreate
        (
            UserId: Guid.NewGuid(),
            AddressId: Guid.NewGuid(),
            OrderProducts: new List<OrderProductToCreate>()
        );

        var createdOrder = new Order(Guid.NewGuid(), orderToCreate.UserId, orderToCreate.AddressId)
        {
            OrderProducts = new List<OrderProduct>()
        };

        _mockMapper.Setup(m => m.Map<Order>(orderToCreate)).Returns(createdOrder).Verifiable(Times.Once);
        _mockOrderService.Setup(s => s.CreateOrderAsync(It.IsAny<Order>())).ReturnsAsync(createdOrder).Verifiable(Times.Once);

        // Act
        var result = await _controller.CreateOrder(orderToCreate);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_controller.GetOrder), createdAtActionResult.ActionName);
        Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
        Assert.Equal(createdOrder, createdAtActionResult.Value);
        _mockMapper.VerifyAll();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllOrders_ReturnsOk_WithListOfOrders()
    {
        // Arrange
        var orders = new List<Order>
        {
            new Order(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()),
            new Order(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
        };

        _mockOrderService.Setup(s => s.GetAllOrdersAsync()).ReturnsAsync(orders).Verifiable(Times.Once);

        // Act
        var result = await _controller.GetAllOrders();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(orders, okResult.Value);
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrder_ReturnsOk_WithOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var order = new Order(orderId, Guid.NewGuid(), Guid.NewGuid());

        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync(order).Verifiable(Times.Once);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(order, okResult.Value);
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrder_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null).Verifiable(Times.Once);

        // Act
        var result = await _controller.GetOrder(orderId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateOrder_ReturnsOk_WithUpdatedOrder()
    {
        // Arrange
        var orderToUpdate = new OrderToUpdate
        (
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            AddressId: Guid.NewGuid(),
            OrderProducts: new List<OrderProductToUpdate>()
        );

        var updatedOrder = new Order(orderToUpdate.Id, orderToUpdate.UserId, orderToUpdate.AddressId);

        _mockMapper.Setup(m => m.Map<Order>(orderToUpdate)).Returns(updatedOrder).Verifiable(Times.Once);
        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderToUpdate.Id)).ReturnsAsync(updatedOrder).Verifiable(Times.Once);
        _mockOrderService.Setup(s => s.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync(updatedOrder).Verifiable(Times.Once);

        // Act
        var result = await _controller.UpdateOrder(orderToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(StatusCodes.Status200OK, okResult.StatusCode);
        Assert.Equal(updatedOrder, okResult.Value);
        _mockMapper.VerifyAll();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateOrder_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderToUpdate = new OrderToUpdate
        (
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            AddressId: Guid.NewGuid(),
            OrderProducts: new List<OrderProductToUpdate>()
            {
                new(Guid.NewGuid(), Guid.NewGuid(), 1, 50.00m)
            }
        );
        var updatedOrder = new Order(orderToUpdate.Id, orderToUpdate.UserId, orderToUpdate.AddressId)
        {
            OrderProducts = new List<OrderProduct> { new(Guid.NewGuid(), Guid.NewGuid(), 2, 10) }
        };
        _mockMapper.Setup(m => m.Map<Order>(orderToUpdate)).Returns(updatedOrder).Verifiable(Times.Once);
        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderToUpdate.Id)).ReturnsAsync((Order?)null).Verifiable(Times.Once);

        // Act
        var result = await _controller.UpdateOrder(orderToUpdate);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockMapper.VerifyAll();
        _mockMapper.VerifyNoOtherCalls();
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteOrder_ReturnsNoContent_WhenOrderIsDeleted()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId))
            .ReturnsAsync(new Order(orderId, Guid.NewGuid(), Guid.NewGuid())).Verifiable(Times.Once);
        _mockOrderService.Setup(s => s.DeleteOrderAsync(orderId)).ReturnsAsync(true).Verifiable(Times.Once);

        // Act
        var result = await _controller.DeleteOrder(orderId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteOrder_ReturnsNotFound_WhenOrderDoesNotExist()
    {
        // Arrange
        var orderId = Guid.NewGuid();

        _mockOrderService.Setup(s => s.GetOrderByIdAsync(orderId)).ReturnsAsync((Order?)null).Verifiable(Times.Once);

        // Act
        var result = await _controller.DeleteOrder(orderId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _mockOrderService.VerifyAll();
        _mockOrderService.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task AddOrderProduct_AddsNewProductToOrder()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var addressId = Guid.NewGuid();

        var order = new Order(orderId, userId, addressId)
        {
            OrderProducts = new List<OrderProduct>()
        };

        var newProduct = new OrderProduct(orderId, Guid.NewGuid(), 2, 20.00m);
        order.OrderProducts.Add(newProduct);

        _mockRepository.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync(order);

        // Act
        var updatedOrder = await _mockRepository.Object.UpdateOrderAsync(order);

        // Assert
        Assert.NotNull(updatedOrder);
        Assert.Single(updatedOrder.OrderProducts);
        Assert.Equal(newProduct.ProductId, updatedOrder.OrderProducts.First().ProductId);
        _mockRepository.Verify(r => r.UpdateOrderAsync(It.IsAny<Order>()), Times.Once);
        _mockRepository.VerifyAll();
        _mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task RemoveOrderProduct_RemovesProductFromOrder()
    {
        // Arrange
        var productToRemove = new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), 1, 50.00m);

        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
        {
            OrderProducts = new List<OrderProduct>
            {
                productToRemove,
                new(Guid.NewGuid(), Guid.NewGuid(), 3, 30.00m)
            }
        };

        order.OrderProducts = order.OrderProducts
            .Where(p => p.ProductId != productToRemove.ProductId)
            .ToList();

        _mockRepository.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync(order);

        // Act
        var updatedOrder = await _mockRepository.Object.UpdateOrderAsync(order);

        // Assert
        Assert.NotNull(updatedOrder);
        Assert.Single(updatedOrder.OrderProducts);
        Assert.DoesNotContain(productToRemove, updatedOrder.OrderProducts);
        _mockRepository.Verify(r => r.UpdateOrderAsync(It.IsAny<Order>()), Times.Once);
        _mockRepository.VerifyAll();
        _mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateOrderProduct_UpdatesProductQuantityAndPrice()
    {
        // Arrange
        var productToUpdate = new OrderProduct(Guid.NewGuid(), Guid.NewGuid(), 1, 50.00m);

        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
        {
            OrderProducts = new List<OrderProduct> { productToUpdate }
        };

        var updatedProduct = productToUpdate with { NumberOfProducts = 5, ProductPrice = 40.00m };

        order.OrderProducts = new List<OrderProduct> { updatedProduct };

        _mockRepository.Setup(r => r.UpdateOrderAsync(It.IsAny<Order>())).ReturnsAsync(order);

        // Act
        var updatedOrder = await _mockRepository.Object.UpdateOrderAsync(order);

        // Assert
        var product = updatedOrder.OrderProducts.FirstOrDefault();
        Assert.NotNull(product);
        Assert.Equal(5, product.NumberOfProducts);
        Assert.Equal(40.00m, product.ProductPrice);
        _mockRepository.Verify(r => r.UpdateOrderAsync(It.IsAny<Order>()), Times.Once);
        _mockRepository.VerifyAll();
        _mockRepository.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetOrderProduct_ReturnsAllProductsForOrder()
    {
        // Arrange
        var order = new Order(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid())
        {
            OrderProducts = new List<OrderProduct>
            {
                new(Guid.NewGuid(), Guid.NewGuid(), 2, 20.00m),
                new(Guid.NewGuid(), Guid.NewGuid(), 3, 30.00m)
            }
        };

        _mockRepository.Setup(r => r.GetOrderByIdAsync(order.Id)).ReturnsAsync(order);

        // Act
        var retrievedOrder = await _mockRepository.Object.GetOrderByIdAsync(order.Id);

        // Assert
        Assert.NotNull(retrievedOrder);
        Assert.Equal(2, retrievedOrder.OrderProducts.Count);
        _mockRepository.Verify(r => r.GetOrderByIdAsync(order.Id), Times.Once);
        _mockRepository.VerifyAll();
        _mockRepository.VerifyNoOtherCalls();
    }
}