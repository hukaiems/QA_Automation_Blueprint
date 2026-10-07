using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class DepositLogsPage : BasePage
{
    private static readonly By PageHeaderLocator =
        By.XPath("//h3[normalize-space()='Deposit Logs']");

    private static readonly By LogRecordsLocator =
        By.XPath(
            "//h3[normalize-space()='Deposit Logs']" +
            "/following-sibling::div[1]//ul[contains(@class,'basis-full')]/li"
        );

    private static readonly By NewestLogRecordLocator =
        By.XPath(
            "(//h3[normalize-space()='Deposit Logs']" +
            "/following-sibling::div[1]//ul[contains(@class,'basis-full')]/li)[1]"
        );

    // These two locators are relative to one <li> log record.
    private static readonly By DateWithinRecordLocator =
        By.XPath(".//p[contains(normalize-space(.), 'Date:')]/span");

    private static readonly By AmountWithinRecordLocator =
        By.XPath(".//p[contains(normalize-space(.), 'Amount:')]/span");

    public DepositLogsPage(
        IWebDriver driver,
        RootSettings settings,
        string accountId)
        : base(driver, settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        PageUrl = BuildUrl(Routes.DepositLogs(accountId));
    }

    public bool IsDepositLogsPageDisplayed()
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
        // Wait until at least the newest record exists before counting.
        _ = GetNewestLogRecord();

        return Driver.FindElements(LogRecordsLocator).Count;
    }

    public string GetNewestLogDate()
    {
        var newestRecord = GetNewestLogRecord();

        return newestRecord
            .FindElement(DateWithinRecordLocator)
            .Text
            .Trim();
    }

    public string GetNewestLogAmount()
    {
        var newestRecord = GetNewestLogRecord();

        return newestRecord
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
                   "No Deposit Logs record was found."
               );
    }
}
