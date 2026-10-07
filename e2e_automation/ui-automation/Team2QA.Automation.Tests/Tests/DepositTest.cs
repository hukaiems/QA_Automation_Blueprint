using System.Globalization;
using Team2QA.Automation.Tests.Pages;
using Team2QA.Automation.Tests.Setup.DepositWithdraw;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
[NonParallelizable]
public class DepositTest : DepositWithdrawFixtureBase
{
    private static readonly HashSet<string>
        SupportedEndToEndScenarios = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            "DEPOSIT_VODAFONE_E2E_SUCCESS",
            "DEPOSIT_CREDIT_CARD_E2E_SUCCESS"
        };

    private static IEnumerable<TestCaseData> DepositEndToEndData()
    {
        var dataList = ExcelUtil.GetTestData(
            "Data/DepositWithdraw.xlsx"
        );

        foreach (var data in dataList)
        {
            if (!data.TryGetValue(
                    "Scenario",
                    out var scenario) ||
                string.IsNullOrWhiteSpace(scenario))
            {
                continue;
            }

            if (!SupportedEndToEndScenarios.Contains(scenario))
            {
                continue;
            }

            yield return new TestCaseData(data)
                .SetName($"Deposit_EndToEnd_{scenario}");
        }
    }

    [Test]
    [Category("Regression")]
    [TestCaseSource(nameof(DepositEndToEndData))]
    [Property("TC_ID", "Deposit-E2E-01")]
    public async Task TC04_Deposit_ShouldIncreaseBalanceAndCreateLog(
        Dictionary<string, string> data)
    {
        var dashboardPage = LoginAsScenarioCustomer();
        var depositPage = dashboardPage.GoToDepositPage();
        var accountId = depositPage.AccountId;

        Scenario.AccountId = accountId;

        var balanceBefore = await Accounts.GetBalanceAsync(
            accountId,
            Scenario.CustomerToken
        );

        var depositAmount = decimal.Parse(
            data["Amount"],
            CultureInfo.InvariantCulture
        );

        var expectedBalance =
            balanceBefore + depositAmount;

        PerformDepositByPaymentMethod(
            depositPage,
            data
        );

        Assert.That(
            depositPage.GetSuccessMessage(),
            Is.EqualTo(data["ExpectedMessage"]),
            "The prerequisite Deposit did not complete successfully."
        );

        var balanceAfter = await BalanceWaiter.WaitForBalanceAsync(
            accountId,
            Scenario.CustomerToken,
            expectedBalance,
            TimeSpan.FromSeconds(15)
        );

        var depositLogsPage = new DepositLogsPage(
            Driver,
            Settings,
            accountId
        );

        depositLogsPage.NavigateTo();

        TestContext.Out.WriteLine(
            $"Deposit Logs URL: {Driver.Url}"
        );

        var actualLogCount =
            depositLogsPage.GetVisibleLogRecordCount();

        var actualAmount =
            depositLogsPage.GetNewestLogAmount();

        var actualDate =
            depositLogsPage.GetNewestLogDate();

        var expectedLogCount = Convert.ToInt32(
            decimal.Parse(
                data["ExpectedLogCount"],
                CultureInfo.InvariantCulture
            )
        );

        var expectedDate = DateTime.Now.ToString(
            "dd-MM-yyyy",
            CultureInfo.InvariantCulture
        );

        TestContext.Out.WriteLine(
            $"Balance before Deposit: {balanceBefore}"
        );

        TestContext.Out.WriteLine(
            $"Balance after Deposit: {balanceAfter}"
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                balanceAfter,
                Is.EqualTo(expectedBalance),
                "The persisted account balance was not increased by the Deposit amount."
            );

            Assert.That(
                actualLogCount,
                Is.EqualTo(expectedLogCount),
                "The number of Deposit Logs records was incorrect."
            );

            Assert.That(
                actualAmount,
                Is.EqualTo(data["ExpectedLogAmount"]),
                "The newest Deposit Logs amount was incorrect."
            );

            Assert.That(
                actualDate,
                Is.EqualTo(expectedDate),
                "The newest Deposit Logs date was incorrect."
            );
        }
    }

    private void PerformDepositByPaymentMethod(
        DepositPage depositPage,
        Dictionary<string, string> data)
    {
        var paymentMethod =
            data["PaymentMethod"];

        if (paymentMethod.Equals(
                "VodafoneCash",
                StringComparison.OrdinalIgnoreCase))
        {
            depositPage.DepositByVodafoneCash(
                amount: data["Amount"],
                accountPassword: Scenario.AccountPassword,
                walletNumber: data["WalletNumber"],
                pin: data["PIN"],
                otp: data["OTP"]
            );

            return;
        }

        if (paymentMethod.Equals(
                "CreditCard",
                StringComparison.OrdinalIgnoreCase))
        {
            depositPage.DepositByCreditCard(
                amount: data["Amount"],
                accountPassword: Scenario.AccountPassword,
                cardNumber: data["CardNumber"],
                nameOnCard: data["NameOnCard"],
                expirationDate: data["ExpirationDate"],
                cvc: data["CVC"]
            );

            return;
        }

        throw new NotSupportedException(
            $"Unsupported payment method: {paymentMethod}"
        );
    }
}
