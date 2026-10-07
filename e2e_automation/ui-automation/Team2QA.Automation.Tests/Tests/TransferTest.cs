using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Pages;
using Team2QA.Automation.Tests.Setup.Transfer;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
public class TransferTest : TransferFixtureBase
{
    private static IEnumerable<TestCaseData> TransferData()
    {
        var dataList = ExcelUtil.GetTestData("Transfer.xlsx", "TestCase");
        foreach (var data in dataList)
        {
            if (string.IsNullOrWhiteSpace(data["Scenario"]))
            {
                continue;
            }

            yield return new TestCaseData(data).SetName($"Transfer_Test_{data["Scenario"]}");
        }
    }
    [Test]
    [Category("Regression")]
    [TestCaseSource(nameof(TransferData))]
    public async Task Transfer_UI_Pos_01(Dictionary<string, string> data)
    {
        var currentDriver = Driver;
        // setup data 
        Scenario = await ScenarioSetup.CreateUsersAsync(data, _adminToken);

        // 2. ACT & ASSERT: Pure UI Testing
        var homePage = new HomePage(currentDriver, Settings);
        var loginPage = homePage.GotoLoginPage();
        var dashboardPage = loginPage.GoToDashBoard(Scenario.Sender.Email, data["Password"]);
        var transferPage = dashboardPage.GoToTransferPage();
        transferPage.FillTransferForm(Scenario.Receiver.AccountRequestId, data["Amount"], data["Password"]);
        int expectedSenderBalance = int.Parse(data["Balance"]) - int.Parse(data["Amount"]);
        int expectedReceiverBalance = int.Parse(data["Balance"]) + int.Parse(data["Amount"]);
        var userApi = new AccountApiClient(_apiClient);
        // remember to await cause the method is async -> promise
        var finalSenderBalance = await ApiWaitUtils.WaitForBalanceUpdateAsync(
            () => userApi.GetBalanceAsync(Scenario.Sender.AccountRequestId, Scenario.Sender.Token),
            expectedSenderBalance
        );

        var finalReceiverBalance = await ApiWaitUtils.WaitForBalanceUpdateAsync(
            () => userApi.GetBalanceAsync(Scenario.Receiver.AccountRequestId, Scenario.Receiver.Token),
            expectedReceiverBalance
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(finalSenderBalance, Is.EqualTo(expectedSenderBalance), "Sender's final balance mismatch.");
            Assert.That(finalReceiverBalance, Is.EqualTo(expectedReceiverBalance), "Receiver's final balance mismatch.");
        }

    }
}
