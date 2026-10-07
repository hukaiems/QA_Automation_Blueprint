using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Models.Scenarios;

namespace Team2QA.Automation.Tests.Setup.DepositWithdraw;

public sealed class DepositWithdrawScenarioSetup
{
    private const decimal DefaultInitialBalance = 5_000_000m;
    private const string DefaultCustomerPassword = "Huynguyen1@2";
    private const string DefaultAccountPassword = "Huynguyen1@2";
    private readonly AuthApiClient _auth;
    private readonly RegisterApiClient _registration;
    private readonly AccountApiClient _accounts;
    private readonly AdminApiClient _administration;

    public DepositWithdrawScenarioSetup(
        AuthApiClient auth,
        RegisterApiClient registration,
        AccountApiClient accounts,
        AdminApiClient administration)
    {
        _auth = auth;
        _registration = registration;
        _accounts = accounts;
        _administration = administration;
    }

    public async Task<string> LoginAdminAsync()
    {
        var response = await _auth.LoginOwnerAsync();
        return response.Token;
    }

    public async Task<DepositWithdrawScenario> CreateApprovedAccountAsync(
        string adminToken,
        decimal initialBalance = DefaultInitialBalance)
    {
        var unique = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");
        var email = $"huy.depositwithdraw.{unique}@gmail.com";
        var phone = $"010{unique[^8..]}";

        var registeredCustomer = await _registration.RegisterCustomerAsync(
            new RegisterCustomerRequest(
                Name: "Huy Automation",
                Email: email,
                Password: DefaultCustomerPassword,
                Address: "this is my address",
                Phone: phone,
                Postal: "12345"
            )
        );

        var customerLogin = await _auth.LoginCustomerAsync(
            email,
            DefaultCustomerPassword
        );

        var accountRequestId = await _accounts.RequestAccountAsync(
            new RequestAccountRequest(
                Balance: initialBalance,
                UserId: registeredCustomer.Id,
                AccountPassword: DefaultAccountPassword,
                CustomerToken: registeredCustomer.Token
            ),
            registeredCustomer.Token
        );

        var approval = await _administration.ApproveAccountAsync(
            new ApproveAccountRequest(
                Balance: initialBalance,
                UserId: registeredCustomer.Id,
                RequestId: long.Parse(accountRequestId),
                AdminToken: adminToken
            ),
            adminToken
        );

        if (approval.Id != accountRequestId)
        {
            throw new InvalidOperationException(
                $"Unexpected approval response ID. " +
                $"Expected: {accountRequestId}; actual: {approval.Id}."
            );
        }

        return new DepositWithdrawScenario
        {
            UserId = registeredCustomer.Id,
            CustomerEmail = email,
            CustomerPassword = DefaultCustomerPassword,
            AccountPassword = DefaultAccountPassword,
            CustomerToken = customerLogin.Token,
            AccountRequestId = accountRequestId,
            InitialBalance = initialBalance
        };
    }

    public Task DeleteCustomerAsync(
        string userId,
        string adminToken)
    {
        return _administration.DeleteCustomerAsync(userId, adminToken);
    }
}
