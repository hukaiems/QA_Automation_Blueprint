using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class WithdrawLogsPage : BasePage
{
    private static readonly By PageHeaderLocator =
        By.XPath("//h3[normalize-space()='Withdraw Logs']");

    private static readonly By LogRecordsLocator =
        By.XPath(
            "//h3[normalize-space()='Withdraw Logs']" +
            "/following-sibling::div[1]//ul[contains(@class,'basis-full')]/li"
        );

    private static readonly By NewestLogRecordLocator =
        By.XPath(
            "(//h3[normalize-space()='Withdraw Logs']" +
            "/following-sibling::div[1]//ul[contains(@class,'basis-full')]/li)[1]"
        );

    private static readonly By DateWithinRecordLocator =
        By.XPath(".//p[contains(normalize-space(.), 'Date:')]/span");

    private static readonly By AmountWithinRecordLocator =
        By.XPath(".//p[contains(normalize-space(.), 'Amount:')]/span");

    public WithdrawLogsPage(
        IWebDriver driver,
        RootSettings settings,
        string accountId)
        : base(driver, settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        PageUrl = BuildUrl(Routes.WithdrawLogs(accountId));
    }

    public bool IsWithdrawLogsPageDisplayed()
    {
        try
        {
            var heading = WaitForElementExist(
                PageHeaderLocator,
                TimeSpan.FromSeconds(10)
            );

            return heading?.Displayed == true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public int GetVisibleLogRecordCount()
    {
        _ = GetNewestLogRecord();

        return Driver.FindElements(LogRecordsLocator).Count;
    }

    public string GetNewestLogDate()
    {
        return GetNewestLogRecord()
            .FindElement(DateWithinRecordLocator)
            .Text
            .Trim();
    }

    public string GetNewestLogAmount()
    {
        return GetNewestLogRecord()
            .FindElement(AmountWithinRecordLocator)
            .Text
            .Trim();
    }

    private IWebElement GetNewestLogRecord()
    {
        return WaitForElementExist(
                   NewestLogRecordLocator,
                   TimeSpan.FromSeconds(10)
               )
               ?? throw new NoSuchElementException(
                   "No Withdraw Logs record was found."
               );
    }
}
