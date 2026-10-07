using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Api;

// use to call every api belong to a user like request account transfering, get balance, etc...
public sealed class AccountApiClient
{
    private readonly ApiClient _client;

    public AccountApiClient(ApiClient client)
    {
        _client = client;
    }

    public async Task<string> RequestAccountAsync(
        RequestAccountRequest request,
        string customerToken, string type = "account-request")
    {
        var response = await _client.PostAsync<
            RequestAccountRequest,
            UserRequestAccountResponse>(
                ApiEndpoints.RequestAccount,
                request,
                customerToken
            );

        var accountRequestId = response.notifications?
            .FirstOrDefault(notification =>
                notification.type == type)?
            .data?
            .FirstOrDefault()?
            .account_id;

        return !string.IsNullOrWhiteSpace(accountRequestId)
            ? accountRequestId
            : throw new InvalidOperationException(
                "Cannot extract the account request ID from the API response."
            );
    }

    public async Task<decimal> GetBalanceAsync(
        string accountId,
        string customerToken)
    {
        var response = await _client.GetAsync<AccountBalanceResponse>(
            ApiEndpoints.AccountInfo(accountId),
            customerToken
        );

        return response.Balance;
    }

}
