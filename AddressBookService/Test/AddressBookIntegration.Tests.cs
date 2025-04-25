using System.Net;
using System.Text;
using AddressBookService.API.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace AddressBookService.Test;

public class AddressBookControllerIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateAddress_ShouldReturnBadRequest_WhenModelIsInvalid_IntegrationTest()
    {
        // Arrange
        var invalidAddressToCreate = new AddressToCreate(
            UserId: Guid.NewGuid(),
            Recipient: "",
            Street: "123 Main Street",
            City: "Anytown",
            State: "CA",
            ZipCode: "",
            Country: "USA",
            Phone: ""
        );

        // Act
        var response = await _client.PostAsJsonAsync("/api/AddressBook", invalidAddressToCreate);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var validationDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(validationDetails);
        AssertValidationError(validationDetails, "Recipient", "Recipient is required.");
        AssertValidationError(validationDetails, "ZipCode", "Zip code is required.");
        AssertValidationError(validationDetails, "Phone", "Phone is required.");
    }

    [Fact]
    public async Task UpdateAddress_ShouldReturnBadRequest_WhenModelIsInvalid_IntegrationTest()
    {
        // Arrange
        const string json = """
                            {
                              "id": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                            "userId": "3fa85f64-5717-4562-b3fc-2c963f66afa6",
                            "recipient": "",
                            "street": "string",
                            "city": "string",
                            "state": "string",
                            "zipCode": "",
                            "country": "string",
                            "phone": ""
                            }
                            """;

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/AddressBook", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var validationDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(validationDetails);
        AssertValidationError(validationDetails, "Recipient", "Recipient is required.");
        AssertValidationError(validationDetails, "ZipCode", "Zip code is required.");
        AssertValidationError(validationDetails, "Phone", "Phone is required.");
    }

    private static void AssertValidationError(ValidationProblemDetails validationDetails, string errorKey,
        string errorMessage)
    {
        Assert.True(validationDetails.Errors.TryGetValue(errorKey, out var error));
        Assert.Equal(errorMessage, error[0]);
    }
}