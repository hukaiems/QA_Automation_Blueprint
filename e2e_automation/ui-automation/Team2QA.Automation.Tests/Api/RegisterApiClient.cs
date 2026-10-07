using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Api;

public sealed class RegisterApiClient
{
    private readonly ApiClient _client;

    public RegisterApiClient(ApiClient client)
    {
        _client = client;
    }

    public Task<RegisterResponse> RegisterCustomerAsync(
        RegisterCustomerRequest request)
    {
        return _client.PostAsync<RegisterCustomerRequest, RegisterResponse>(
            ApiEndpoints.UserRegister,
            request
        );
    }
    public Task<RegisterAdminResponse> RegisterAdminAsync(
        RegisterAdminRequest request)
    {
        return _client.PostAsync<RegisterAdminRequest, RegisterAdminResponse>(
            ApiEndpoints.AdminRegister,
            request,
            request.Token
        );
    }
}
