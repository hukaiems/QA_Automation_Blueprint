using System.Globalization;
using Team2QA.Automation.Tests.Pages;
using Team2QA.Automation.Tests.Setup.DepositWithdraw;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
[NonParallelizable]
public class WithdrawTest : DepositWithdrawFixtureBase
{
    private static readonly HashSet<string>
        SupportedEndToEndScenarios = new(
            StringComparer.OrdinalIgnoreCase
        )
        {
            "WITHDRAW_VODAFONE_SUCCESS",
            "WITHDRAW_CREDIT_CARD_SUCCESS"
        };

    private static IEnumerable<TestCaseData> WithdrawEndToEndData()
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
                .SetName($"Withdraw_EndToEnd_{scenario}");
        }
    }

    [Test]
    [Category("Regression")]
    [TestCaseSource(nameof(WithdrawEndToEndData))]
    [Property("TC_ID", "Withdraw-E2E-01")]
    public async Task TC04_Withdraw_ShouldDecreaseBalanceAndCreateLog(
        Dictionary<string, string> data)
    {
        var dashboardPage = LoginAsScenarioCustomer();
        var withdrawPage = dashboardPage.GoToWithdrawPage();
        var accountId = withdrawPage.AccountId;

        Scenario.AccountId = accountId;

        var balanceBefore = await Accounts.GetBalanceAsync(
            accountId,
            Scenario.CustomerToken
        );

        var withdrawAmount = decimal.Parse(
            data["Amount"],
            CultureInfo.InvariantCulture
        );

        var expectedBalance =
            balanceBefore - withdrawAmount;

        PerformWithdrawByPaymentMethod(
            withdrawPage,
            data
        );

        Assert.That(
            withdrawPage.GetSuccessMessage(),
            Is.EqualTo(data["ExpectedMessage"]),
            "The Withdraw operation did not display the expected success message."
        );

        var balanceAfter = await BalanceWaiter.WaitForBalanceAsync(
            accountId,
            Scenario.CustomerToken,
            expectedBalance,
            TimeSpan.FromSeconds(15)
        );

        var withdrawLogsPage = new WithdrawLogsPage(
            Driver,
            Settings,
            accountId
        );

        withdrawLogsPage.NavigateTo();

        TestContext.Out.WriteLine(
            $"Withdraw Logs URL: {Driver.Url}"
        );

        Assert.That(
            withdrawLogsPage.IsWithdrawLogsPageDisplayed(),
            Is.True,
            $"Withdraw Logs page was not displayed. Current URL: {Driver.Url}"
        );

        var actualLogCount =
            withdrawLogsPage.GetVisibleLogRecordCount();

        var actualAmount =
            withdrawLogsPage.GetNewestLogAmount();

        var actualDate =
            withdrawLogsPage.GetNewestLogDate();

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
            $"Balance before Withdraw: {balanceBefore}"
        );

        TestContext.Out.WriteLine(
            $"Balance after Withdraw: {balanceAfter}"
        );

        using (Assert.EnterMultipleScope())
        {
            Assert.That(
                balanceAfter,
                Is.EqualTo(expectedBalance),
                "The persisted account balance was not decreased by the Withdraw amount."
            );

            Assert.That(
                actualLogCount,
                Is.EqualTo(expectedLogCount),
                "The number of Withdraw Logs records was incorrect."
            );

            Assert.That(
                actualAmount,
                Is.EqualTo(data["ExpectedLogAmount"]),
                "The newest Withdraw Logs amount was incorrect."
            );

            Assert.That(
                actualDate,
                Is.EqualTo(expectedDate),
                "The newest Withdraw Logs date was incorrect."
            );
        }
    }

    private void PerformWithdrawByPaymentMethod(
        WithdrawPage withdrawPage,
        Dictionary<string, string> data)
    {
        var paymentMethod = data["PaymentMethod"].Trim();

        if (paymentMethod.Equals(
                "VodafoneCash",
                StringComparison.OrdinalIgnoreCase))
        {
            withdrawPage.WithdrawByVodafoneCash(
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
            withdrawPage.WithdrawByCreditCard(
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
            $"Unsupported Withdraw payment method: {paymentMethod}"
        );
    }
}
