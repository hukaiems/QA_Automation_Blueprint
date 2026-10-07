using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Api;

public sealed class AuthApiClient
{
    private readonly ApiClient _client;
    private readonly RootSettings _rootSettings = ConfigReader.Instance;

    public AuthApiClient(ApiClient client)
    {
        _client = client;
    }

    public Task<AuthResponse> LoginAdminAsync(
        string email,
        string password)
    {
        return LoginAsync(ApiEndpoints.AdminLogin, email, password);
    }

    public Task<AuthResponse> LoginCustomerAsync(
        string email,
        string password)
    {
        return LoginAsync(ApiEndpoints.UserLogin, email, password);
    }

    private Task<AuthResponse> LoginAsync(
        string endpoint,
        string email,
        string password)
    {
        return _client.PostAsync<LoginRequest, AuthResponse>(
            endpoint,
            new LoginRequest(email, password)
        );
    }
    public async Task<AuthResponse> LoginOwnerAsync() => await LoginAdminAsync(_rootSettings.Admin.Username, _rootSettings.Admin.Password);

}
