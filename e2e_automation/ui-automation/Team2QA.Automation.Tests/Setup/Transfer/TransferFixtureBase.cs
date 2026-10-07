using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.Scenarios;
using Team2QA.Automation.Tests.Tests;

namespace Team2QA.Automation.Tests.Setup.Transfer;

// Inherit from BaseTest to get Driver and setting from DriverFactory
public abstract class TransferFixtureBase : BaseTest // abstract -> only run by child execution
{
    protected ApiClient _apiClient = null!; // for setting up API
    protected string _adminToken = string.Empty;

    // protected so test script can read
    // set to null so only test run those obj would get instantiated.
    protected TransferScenario Scenario { get; set; } = null!;
    protected TransferScenarioSetup ScenarioSetup { get; set; } = null!;

    [OneTimeSetUp]
    public async Task TransferOneTimeSetUp()
    {
        _apiClient = new ApiClient(Settings.AppSettings.BaseUrl);

        // instantiate those api tools
        var adminApi = new AdminApiClient(_apiClient);
        var registerApi = new RegisterApiClient(_apiClient);
        var accountApi = new AccountApiClient(_apiClient);
        var authApi = new AuthApiClient(_apiClient);

        ScenarioSetup = new TransferScenarioSetup(adminApi, registerApi, accountApi);

        // 3. Login Admin once for all tests in this file
        var adminLogin = await authApi.LoginAdminAsync("admin2@gmail.com", "P@ssw0rdP@ssw0rd");
        _adminToken = adminLogin.Token;
    }

    [TearDown]
    public async Task TransferTearDown()
    {
        //5. Delete both users
        if (Scenario != null && !string.IsNullOrWhiteSpace(_adminToken))
        {
            await ScenarioSetup.CleanupAsync(Scenario.Sender.Id, Scenario.Receiver.Id, _adminToken);
            System.Console.WriteLine("Cleaned up both users!");
        }
    }

    [OneTimeTearDown]
    public void TransferOneTimeTearDown()
    {
        _apiClient?.Dispose();
    }
}