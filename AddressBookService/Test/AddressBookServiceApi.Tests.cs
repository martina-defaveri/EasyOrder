using AddressBookService.API;
using AddressBookService.API.DTO;
using AddressBookService.Application;
using AddressBookService.Domain;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AddressBookService.Test;

public class AddressServiceApiTests
{
    private readonly Mock<IAddressBookService> _addressServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AddressBookController _addressController;

    public AddressServiceApiTests()
    {
        _addressServiceMock = new Mock<IAddressBookService>();
        _mapperMock = new Mock<IMapper>();
        _addressController = new AddressBookController(_addressServiceMock.Object, _mapperMock.Object);
    }

    [Fact]
    public async Task CreateAddress_ShouldReturnCreatedAtAction_WhenAddressIsCreated()
    {
        // Arrange
        var addressToCreate = new AddressToCreate(
            UserId: Guid.NewGuid(),
            Recipient: "John Doe",
            Street: "123 Main Street",
            City: "Anytown",
            State: "CA",
            ZipCode: "90210",
            Country: "USA",
            Phone: "123-456-7890"
        );
        var mappedAddress = new Address(
            Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Recipient: "John Doe",
            Street: "123 Main Street",
            City: "Anytown",
            State: "CA",
            ZipCode: "90210",
            Country: "USA",
            Phone: "123-456-7890");
        _mapperMock.Setup(m => m.Map<Address>(addressToCreate)).Returns(mappedAddress).Verifiable(Times.Once);
        _addressServiceMock.Setup(s => s.CreateAddressAsync(mappedAddress)).ReturnsAsync(mappedAddress)
            .Verifiable(Times.Once);

        // Act
        var result = await _addressController.CreateAddress(addressToCreate);

        // Assert
        var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(nameof(_addressController.GetAddress), createdAtActionResult.ActionName);
        Assert.Equal(mappedAddress, createdAtActionResult.Value);
        _mapperMock.VerifyAll();
        _mapperMock.VerifyNoOtherCalls();
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAllAddresses_ShouldReturnOkResult_WithListOfAddresses()
    {
        // Arrange
        var addresses = new List<Address>
        {
            new(Id: Guid.NewGuid(),
                UserId: Guid.NewGuid(),
                Recipient: "John Doe",
                Street: "123 Main Street",
                City: "Anytown",
                State: "CA",
                ZipCode: "90210",
                Country: "USA",
                Phone: "123-456-7890"),
            new(
                Id: Guid.NewGuid(),
                UserId: Guid.NewGuid(),
                Recipient: "Jane Smith",
                Street: "456 Elm Street",
                City: "Springfield",
                State: "TX",
                ZipCode: "75001",
                Country: "USA",
                Phone: "987-654-3210"
            ),
        };
        _addressServiceMock.Setup(s => s.GetAllAddressesAsync()).ReturnsAsync(addresses).Verifiable(Times.Once);

        // Act
        var result = await _addressController.GetAllAddresses();

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(addresses, okResult.Value);
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAddress_ShouldReturnOkResult_WhenAddressExists()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var address = new Address(Id: addressId,
            UserId: Guid.NewGuid(),
            Recipient: "John Doe",
            Street: "123 Main Street",
            City: "Anytown",
            State: "CA",
            ZipCode: "90210",
            Country: "USA",
            Phone: "123-456-7890");
        _addressServiceMock.Setup(s => s.GetAddressByIdAsync(addressId)).ReturnsAsync(address).Verifiable(Times.Once);

        // Act
        var result = await _addressController.GetAddress(addressId);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.Equal(address, okResult.Value);
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAddress_ShouldReturnNotFound_WhenAddressDoesNotExist()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        _addressServiceMock.Setup(s => s.GetAddressByIdAsync(addressId)).ReturnsAsync((Address?)null)
            .Verifiable(Times.Once);

        // Act
        var result = await _addressController.GetAddress(addressId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAddress_ShouldRaiseAnException_WhenServiceIsInError()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        _addressServiceMock.Setup(s => s.GetAddressByIdAsync(addressId)).ThrowsAsync(new Exception("ServiceError"))
            .Verifiable(Times.Once);

        // Act
        async Task Logic() => await _addressController.GetAddress(addressId);

        // Assert
        var caught = await Assert.ThrowsAsync<Exception>(Logic);
        Assert.NotNull(caught);
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturnOkResult_WhenAddressIsUpdated()
    {
        // Arrange
        var addressToUpdate = new AddressToUpdate(Id: Guid.NewGuid(),
            UserId: Guid.NewGuid(),
            Recipient: "John Doe",
            Street: "123 Main Street",
            City: "Anytown",
            State: "CA",
            ZipCode: "90210",
            Country: "USA",
            Phone: "123-456-7890");
        var oldAddress = new Address(addressToUpdate.Id, addressToUpdate.UserId, "OldRecipient",
            addressToUpdate.Street, addressToUpdate.City, addressToUpdate.State, addressToUpdate.ZipCode,
            addressToUpdate.Country, addressToUpdate.Phone);
        var updatedAddress = new Address(addressToUpdate.Id, addressToUpdate.UserId, "NewRecipient",
            addressToUpdate.Street, addressToUpdate.City, addressToUpdate.State, addressToUpdate.ZipCode,
            addressToUpdate.Country, addressToUpdate.Phone);
        _mapperMock.Setup(m => m.Map<Address>(addressToUpdate)).Returns(updatedAddress);
        _addressServiceMock.Setup(s => s.GetAddressByIdAsync(addressToUpdate.Id)).ReturnsAsync(oldAddress)
            .Verifiable(Times.Once);
        _addressServiceMock.Setup(s => s.UpdateAddressAsync(It.Is<Address>(x => x.Recipient == "NewRecipient")))
            .ReturnsAsync(updatedAddress).Verifiable(Times.Once);

        // Act
        var result = await _addressController.UpdateAddress(addressToUpdate);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(result);
        var okObjectResult = Assert.IsType<Address>(okResult.Value);
        Assert.Equal(updatedAddress.Recipient, okObjectResult.Recipient);
        _mapperMock.VerifyAll();
        _mapperMock.VerifyNoOtherCalls();
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturnNoContentResult_WhenAddressIsDeleted()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        var address = new Address(Id: addressId,
            UserId: Guid.NewGuid(),
            Recipient: "John Doe",
            Street: "123 Main Street",
            City: "Anytown",
            State: "CA",
            ZipCode: "90210",
            Country: "USA",
            Phone: "123-456-7890");
        _addressServiceMock.Setup(s => s.GetAddressByIdAsync(addressId)).ReturnsAsync(address).Verifiable(Times.Once);
        _addressServiceMock.Setup(s => s.DeleteAddressAsync(addressId)).Returns(Task.CompletedTask)
            .Verifiable(Times.Once);

        // Act
        var result = await _addressController.DeleteAddress(addressId);

        // Assert
        Assert.IsType<NoContentResult>(result);
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task DeleteAddress_ShouldReturnNotFoundResult_WhenAddressDoesNotExist()
    {
        // Arrange
        var addressId = Guid.NewGuid();
        _addressServiceMock.Setup(s => s.GetAddressByIdAsync(addressId)).ReturnsAsync((Address?)null)
            .Verifiable(Times.Once);

        // Act
        var result = await _addressController.DeleteAddress(addressId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
        _addressServiceMock.VerifyAll();
        _addressServiceMock.VerifyNoOtherCalls();
    }
}