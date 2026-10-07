namespace Team2QA.Automation.Tests.Api;

public sealed class AccountBalanceWaiter
{
    private readonly AccountApiClient _accounts;

    public AccountBalanceWaiter(AccountApiClient accounts)
    {
        _accounts = accounts;
    }

    public async Task<decimal> WaitForBalanceAsync(
        string accountId,
        string customerToken,
        decimal expectedBalance,
        TimeSpan timeout)
    {
        var deadline = DateTime.UtcNow + timeout;
        var actualBalance = await _accounts.GetBalanceAsync(
            accountId,
            customerToken
        );

        while (actualBalance != expectedBalance &&
               DateTime.UtcNow < deadline)
        {
            await Task.Delay(TimeSpan.FromMilliseconds(500));

            actualBalance = await _accounts.GetBalanceAsync(
                accountId,
                customerToken
            );
        }

        return actualBalance;
    }
}
