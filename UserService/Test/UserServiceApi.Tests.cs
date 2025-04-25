using Xunit;
using Moq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using UserService.API;
using UserService.API.DTO;
using UserService.Application;
using UserService.Domain;

namespace UserService.Test;

public class UserServiceApiTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserController _userController;

    public UserServiceApiTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _mapperMock = new Mock<IMapper>();
        _userController = new UserController(_userServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateUser_ShouldReturnCreatedAtAction_WhenUserIsCreated()
    {
        // Arrange
        var userToCreate = new UserToCreate("John", "Doe", "test@test.test", "password");
        var mappedUser = new User(Guid.NewGuid(), "John", "Doe", "test@test.test", "password");
        _mapperMock.Setup(m => m.Map<User>(userToCreate)).Returns(mappedUser).Verifiable(Times.Once);
        _userServiceMock.Setup(s => s.CreateUserAsync(mappedUser)).ReturnsAsync(mappedUser).Verifiable(Times.Once);

        // Act
        var result = await _userController.CreateUser(userToCreate);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_userController.GetUser), createdAtActionResult.ActionName);
        Assert.Equal(mappedUser, createdAtActionResult.Value);
        _mapperMock.VerifyAll();
        _mapperMock.VerifyNoOtherCalls();
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllUsers_ShouldReturnOkResult_WithListOfUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User(Guid.NewGuid(), "John", "Doe", "john@example.com", "Password1"),
            new User(Guid.NewGuid(), "Jane", "Doe", "jane@example.com", "Password2")
        };
        _userServiceMock.Setup(s => s.GetAllUsersAsync()).ReturnsAsync(users).Verifiable(Times.Once);

        // Act
        var result = await _userController.GetAllUsers();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(users, okResult.Value);
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUser_ShouldReturnOkResult_WhenUserExists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(userId, "John", "Doe", "test@test.test", "password");
        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user).Verifiable(Times.Once);

        // Act
        var result = await _userController.GetUser(userId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(user, okResult.Value);
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetUser_ShouldReturnNotFound_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User?)null).Verifiable(Times.Once);

        // Act
        var result = await _userController.GetUser(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }
    
    [Fact]
    public async Task GetUser_ShouldRaiseAnException_WhenServiceIsInError()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ThrowsAsync(new Exception("ServiceError")).Verifiable(Times.Once);

        // Act
        async Task Logic() => await _userController.GetUser(userId);

        // Assert
        var caught = await Assert.ThrowsAsync<Exception>(Logic);
        Assert.NotNull(caught);
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnOkResult_WhenUserIsUpdated()
    {
        // Arrange
        var userToUpdate = new UserToUpdate(Guid.NewGuid(), "John", "Updated", "updated@example.com", "Password2");
        var oldUser = new User(userToUpdate.Id, userToUpdate.FirstName, "OldLastName", userToUpdate.Email, userToUpdate.Password);
        var updatedUser = new User(userToUpdate.Id, userToUpdate.FirstName, "Updated", userToUpdate.Email, userToUpdate.Password);
        _mapperMock.Setup(m => m.Map<User>(userToUpdate)).Returns(updatedUser);
        _userServiceMock.Setup(s => s.GetUserByIdAsync(userToUpdate.Id)).ReturnsAsync(oldUser).Verifiable(Times.Once);
        _userServiceMock.Setup(s => s.UpdateUserAsync(It.Is<User>(x=> x.LastName == "Updated"))).ReturnsAsync(updatedUser).Verifiable(Times.Once);

        // Act
        var result = await _userController.UpdateUser(userToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var okObjectResult = Assert.IsType<User>(okResult.Value);
        Assert.Equal(updatedUser.LastName, okObjectResult.LastName);
        _mapperMock.VerifyAll();
        _mapperMock.VerifyNoOtherCalls();
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNoContentResult_WhenUserIsDeleted()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User(userId, "John", "Doe", "test@test.test", "password");
        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync(user).Verifiable(Times.Once);
        _userServiceMock.Setup(s => s.DeleteUserAsync(userId)).Returns(Task.CompletedTask).Verifiable(Times.Once);

        // Act
        var result = await _userController.DeleteUser(userId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteUser_ShouldReturnNotFoundResult_WhenUserDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _userServiceMock.Setup(s => s.GetUserByIdAsync(userId)).ReturnsAsync((User?)null).Verifiable(Times.Once);

        // Act
        var result = await _userController.DeleteUser(userId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _userServiceMock.VerifyAll();
        _userServiceMock.VerifyNoOtherCalls();
    }
}