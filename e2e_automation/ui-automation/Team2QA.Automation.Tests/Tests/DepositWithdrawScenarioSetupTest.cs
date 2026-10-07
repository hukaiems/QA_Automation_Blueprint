using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Setup.DepositWithdraw;
namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
[NonParallelizable]
public class DepositWithdrawScenarioSetupTest
{
    [Test]
    public async Task ScenarioSetup_ShouldCreateAndDeleteApprovedCustomerAccount()
    {
        using var transport = new ApiClient();

        var scenarioSetup = new DepositWithdrawScenarioSetup(
            new AuthApiClient(transport),
            new RegisterApiClient(transport),
            new AccountApiClient(transport),
            new AdminApiClient(transport)
        );

        var adminToken = await scenarioSetup.LoginAdminAsync();
        var scenario = await scenarioSetup.CreateApprovedAccountAsync(
            adminToken
        );

        try
        {
            Assert.That(scenario.UserId, Is.Not.Empty);
            Assert.That(scenario.CustomerEmail, Is.Not.Empty);
            Assert.That(scenario.CustomerToken, Is.Not.Empty);
            Assert.That(scenario.AccountRequestId, Is.Not.Empty);

            TestContext.Out.WriteLine(
                $"Generated user id: {scenario.UserId}"
            );
            TestContext.Out.WriteLine(
                $"Generated email: {scenario.CustomerEmail}"
            );
            TestContext.Out.WriteLine(
                $"Generated account request id: {scenario.AccountRequestId}"
            );
        }
        finally
        {
            await scenarioSetup.DeleteCustomerAsync(
                scenario.UserId,
                adminToken
            );
        }
    }
}
