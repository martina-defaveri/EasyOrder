using System.Net;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using UserService.API.DTO;
using Xunit;

namespace UserService.Test;

public class UserControllerIntegrationTests(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task CreateUser_ShouldReturnBadRequest_WhenModelIsInvalid_IntegrationTest()
    {
        // Arrange
        var invalidUserToCreate = new UserToCreate(string.Empty, "Doe", "invalid-email", "");

        // Act
        var response = await _client.PostAsJsonAsync("/api/User", invalidUserToCreate);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var validationDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(validationDetails);
        AssertValidationError(validationDetails, "FirstName", "First name is required.");
        AssertValidationError(validationDetails, "Email", "Invalid email address.");
        AssertValidationError(validationDetails, "Password", "Password is required.");
    }

    [Fact]
    public async Task UpdateUser_ShouldReturnBadRequest_WhenModelIsInvalid_IntegrationTest()
    {
        // Arrange
        const string json = """
                            {
                              "id": "51b815fd-afb0-434c-bb76-369009250219",
                              "firstName": "",
                              "lastName": "Doe",
                              "email": "test@",
                              "password": "1234567"
                            }
                            """;

        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PutAsync("/api/User", content);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var validationDetails = await response.Content.ReadFromJsonAsync<ValidationProblemDetails>();
        Assert.NotNull(validationDetails);
        AssertValidationError(validationDetails, "FirstName", "First name is required.");
        AssertValidationError(validationDetails, "Email", "Invalid email address.");
        AssertValidationError(validationDetails, "Password", "Password must be at least 8 characters long.");
    }

    private static void AssertValidationError(ValidationProblemDetails validationDetails, string errorKey,
        string errorMessage)
    {
        Assert.True(validationDetails.Errors.TryGetValue(errorKey, out var error));
        Assert.Equal(errorMessage, error[0]);
    }
}