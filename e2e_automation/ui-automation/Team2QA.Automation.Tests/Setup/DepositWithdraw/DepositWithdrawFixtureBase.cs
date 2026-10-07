using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.Scenarios;
using Team2QA.Automation.Tests.Pages;
using Team2QA.Automation.Tests.Tests;

namespace Team2QA.Automation.Tests.Setup.DepositWithdraw;

public abstract class DepositWithdrawFixtureBase : BaseTest
{
    private ApiClient _transport = null!;
    private string _adminToken = string.Empty;

    protected DepositWithdrawScenario Scenario { get; private set; } = null!;
    protected AccountApiClient Accounts { get; private set; } = null!;
    protected AccountBalanceWaiter BalanceWaiter { get; private set; } = null!;
    protected DepositWithdrawScenarioSetup ScenarioSetup { get; private set; } = null!;

    [OneTimeSetUp]
    public async Task DepositWithdrawOneTimeSetUp()
    {
        _transport = new ApiClient();

        var authentication = new AuthApiClient(_transport);
        var registration = new RegisterApiClient(_transport);
        Accounts = new AccountApiClient(_transport);
        var administration = new AdminApiClient(_transport);

        BalanceWaiter = new AccountBalanceWaiter(Accounts);
        ScenarioSetup = new DepositWithdrawScenarioSetup(
            authentication,
            registration,
            Accounts,
            administration
        );

        _adminToken = await ScenarioSetup.LoginAdminAsync();

        Assert.That(
            _adminToken,
            Is.Not.Empty,
            "Admin login API did not return a token."
        );
    }

    [SetUp]
    public async Task DepositWithdrawSetUp()
    {
        Scenario = await ScenarioSetup.CreateApprovedAccountAsync(
            _adminToken
        );

        TestContext.Out.WriteLine(
            $"Generated customer email: {Scenario.CustomerEmail}"
        );
        TestContext.Out.WriteLine(
            $"Generated user ID: {Scenario.UserId}"
        );
        TestContext.Out.WriteLine(
            $"Generated account request ID: {Scenario.AccountRequestId}"
        );
        TestContext.Out.WriteLine(
            $"Initial balance: {Scenario.InitialBalance}"
        );
    }

    [TearDown]
    public async Task DepositWithdrawTearDown()
    {
        if (Scenario is null ||
            string.IsNullOrWhiteSpace(Scenario.UserId) ||
            string.IsNullOrWhiteSpace(_adminToken))
        {
            TestContext.Out.WriteLine(
                "Cleanup skipped because generated user information was unavailable."
            );
            return;
        }

        try
        {
            await ScenarioSetup.DeleteCustomerAsync(
                Scenario.UserId,
                _adminToken
            );

            TestContext.Out.WriteLine(
                $"Deleted generated user: {Scenario.UserId}"
            );
        }
        catch (Exception exception)
        {
            TestContext.Out.WriteLine(
                $"Deposit/Withdraw cleanup failed: {exception.Message}"
            );
        }
    }

    [OneTimeTearDown]
    public void DepositWithdrawOneTimeTearDown()
    {
        _transport?.Dispose();
    }

    protected DashboardPage LoginAsScenarioCustomer()
    {
        var homePage = new HomePage(Driver, Settings);
        var loginPage = homePage.GotoLoginPage();

        return loginPage.GoToDashBoard(
            Scenario.CustomerEmail,
            Scenario.CustomerPassword
        );
    }
}
