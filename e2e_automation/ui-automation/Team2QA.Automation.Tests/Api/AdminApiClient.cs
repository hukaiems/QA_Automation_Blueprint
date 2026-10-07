using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Api;

public sealed class AdminApiClient
{
    /*
    Dependency Injection
    - Create readonly var
    - Pass down, inject it to the constructor, many classes can use the same ApiClient instance.
    
    */
    private readonly ApiClient _client;

    public AdminApiClient(ApiClient client)
    {
        // catch the injected var and save into internal var in constructor
        _client = client;
    }
    // using the task so it give back a Promise object for async job
    public Task<ApproveResponse> ApproveAccountAsync(
        ApproveAccountRequest request,
        string adminToken)
    {
        return _client.PostAsync<ApproveAccountRequest, ApproveResponse>(
            ApiEndpoints.ApproveRequest,
            request,
            adminToken
        );
    }

    public Task DeleteCustomerAsync(
        string userId,
        string adminToken)
    {
        if (string.IsNullOrWhiteSpace(userId))
        {
            return Task.CompletedTask;
        }

        return _client.DeleteAsync(
            ApiEndpoints.DeleteUser(userId),
            adminToken
        );
    }
    public Task<DeleteUserResponse> DeleteAdminAsync(
        string adminId,
        string ownerToken)
    {
        return _client.DeleteAsync<DeleteUserResponse>(
            ApiEndpoints.DeleteAdmin(adminId),
            ownerToken
        );
    }

}
