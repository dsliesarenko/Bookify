using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Infrastructure;
using FluentAssertions;
using System.Net;
using System.Net.Http.Json;

namespace Bookify.Api.FunctionalTests.Users;

public class RegisterUserTests : BaseFunctionalTest
{
    public RegisterUserTests(FunctionalTestWebAppFactory factory) : base(factory)
    {
    }

    [Fact]
    public async Task Register_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterUserRequest("create@test.com", "first", "last", "12345");

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Theory]
    [InlineData("", "first", "last", "12345")]
    [InlineData("test.com", "first", "last", "12345")]
    [InlineData("@test.com", "first", "last", "12345")]
    public async Task Register_ShouldReturnBadRequest_WhenRequestIsInvalid(string email, string firstName, string lastName, string password)
    {
        // Arrange
        var request = new RegisterUserRequest(email, firstName, lastName, password);

        // Act
        var response = await httpClient.PostAsJsonAsync("api/v1/users/register", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
