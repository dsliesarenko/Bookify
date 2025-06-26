using Bookify.Api.Controllers.Users;
using Bookify.Api.FunctionalTests.Users;
using Bookify.Application.Users.LogInUser;
using System.Net.Http.Json;

namespace Bookify.Api.FunctionalTests.Infrastructure;

public abstract class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    protected readonly HttpClient httpClient;

    protected BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        httpClient = factory.CreateClient();
    }

    protected async Task<string> GetAccessToken()
    {
        HttpResponseMessage loginResponse = await httpClient.PostAsJsonAsync("api/v1/users/login", new LogInUserRequest(UserData.RegisterTestUserRequest.Email, UserData.RegisterTestUserRequest.Password));

        var accessTokenResponse = await loginResponse.Content.ReadFromJsonAsync<AccessTokenResponse>();

        return accessTokenResponse!.AccessToken;
    }
}
