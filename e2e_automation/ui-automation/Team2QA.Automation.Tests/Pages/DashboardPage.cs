using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Models.UIData;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class DashboardPage : BasePage
{
    private static readonly By AccountIdLocator = By.XPath("//p[normalize-space(text())='Account ID']/following-sibling::*[1]");
    private static readonly By CreatedAtLocator = By.XPath("//p[normalize-space(text())='Created At']/following-sibling::*[1]");
    private static readonly By BalanceLocator = By.XPath("//p[normalize-space(text())='Balance']/following-sibling::*[1]");
    private static readonly By OutGoingTransactionLocator = By.XPath("//p[normalize-space(text())='OutGoing Transcations']/following-sibling::*[1]");
    private static readonly By InComingTransactionLocator = By.XPath("//p[normalize-space(text())='Incoming Transcations']/following-sibling::*[1]");
    private static readonly By DepositAmtLocator = By.XPath("//p[normalize-space(text())='Deposit Amount']/following-sibling::*[1]");
    private static readonly By WithdrawAmtLocator = By.XPath("//p[normalize-space(text())='Withdrawal Amount']/following-sibling::*[1]");
    private static readonly By UserTableLocator = By.TagName("table");
    private static readonly By ProfileLinkLocator = By.PartialLinkText("Profile");
    private static readonly By AddNewAdminLinkLocator = By.XPath("//li[normalize-space()='Add New Admins']");
    private static readonly By LogoutLinkLocator = By.XPath("//button[.//span[normalize-space()='Logout']]");
    private static readonly By ReqAccountLinkLocator = By.XPath("(//span[normalize-space()='Request Account'])[1]");
    private static readonly By NotificationTabLocator = By.XPath("//span[normalize-space()='Notifications']");
    private static readonly By AccountRequestItemLocator = By.XPath("(//a[contains(@href, '/notifications/')][descendant::*[contains(text(), 'Account Request')]])[1]");
    private static readonly By FirstNotificationIdLocator = By.XPath("//p[contains(., 'Account Request Id')]//span");
    private static readonly By DepositLinkLocator = By.CssSelector("a[href*='/account/deposit/']");
    private static readonly By WithdrawLinkLocator = By.CssSelector("a[href*='/account/withdraw/']");
    private static readonly By HomeLinkLocator = By.XPath("//a[.//span[normalize-space()='Home']]");
    private static readonly By SettingLinkLocator = By.XPath("//a[.//span[normalize-space()='Setting']]");
    private static readonly By OutGoingBalanceLinkLocator = By.XPath("//a[.//span[normalize-space()='OutGoing Balance']]");
    private static readonly By IncomingBalanceLinkLocator = By.XPath("//a[.//span[normalize-space()='Incoming Balance']]");
    private static readonly By DepositLogsLinkLocator = By.XPath("//a[.//span[normalize-space()='Deposit Logs']]");
    private static readonly By WithdrawLogsLinkLocator = By.XPath("//a[.//span[normalize-space()='Withdraw Logs']]");
    private static readonly By TransferLinkLocator = By.XPath("//a[.//span[normalize-space()='Transfer']]");
    private static readonly By WelcomeMessageLocator = By.XPath("//span[contains(text(), 'Welcome,')]");
    public DashboardPage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.Dashboard);
    }
    public AccountDetail GetAccountDetail()
    {
        return new AccountDetail(
            WaitForElementExist(AccountIdLocator)?.Text ?? string.Empty,
            WaitForElementExist(CreatedAtLocator)?.Text ?? string.Empty,
            WaitForElementExist(BalanceLocator)?.Text ?? string.Empty,
            WaitForElementExist(OutGoingTransactionLocator)?.Text ?? string.Empty,
            WaitForElementExist(InComingTransactionLocator)?.Text ?? string.Empty,
            WaitForElementExist(DepositAmtLocator)?.Text ?? string.Empty,
            WaitForElementExist(WithdrawAmtLocator)?.Text ?? string.Empty
        );
    }
    public string GetUserId()
    {
        var profileLink = WaitForClickable(ProfileLinkLocator);
        var userId = profileLink.GetAttribute("href")?.Split("/").Last();
        return userId ?? string.Empty;
    }
    public int GetTableCount()
    {
        var table = WaitForElementExist(UserTableLocator);
        var rows = table?.FindElements(By.TagName("tr"));
        return rows?.Count ?? 0;
    }
    public ProfilePage GoToProfile()
    {
        var profileLink = WaitForClickable(ProfileLinkLocator);
        var userId = profileLink.GetAttribute("href")?.Split("/").Last();
        Click(profileLink);
        return new ProfilePage(Driver, rootSettings, userId ?? string.Empty);
    }
    public void Logout()
    {
        var logoutLink = WaitForClickable(LogoutLinkLocator);
        Click(logoutLink);
    }
    public bool IsProfileLinkDisplayed()
    {
        try
        {
            var profileLink = WaitForClickable(ProfileLinkLocator);
            return profileLink.Displayed ? true : false;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public TransferPage GoToTransferPage()
    {
        var transferLink = WaitForClickable(TransferLinkLocator);
        Click(transferLink);
        var userId = transferLink.GetAttribute("href")?.Split("/").Last();
        return new TransferPage(Driver, rootSettings, userId ?? string.Empty);
    }

    public DepositPage GoToDepositPage()
    {
        var depositLink = WaitForClickable(DepositLinkLocator);
        var accountId = GetRouteIdentifier(depositLink);
        Click(depositLink);

        return new DepositPage(Driver, rootSettings, accountId);
    }

    public WithdrawPage GoToWithdrawPage()
    {
        var withdrawLink = WaitForClickable(WithdrawLinkLocator);
        var accountId = GetRouteIdentifier(withdrawLink);
        Click(withdrawLink);

        return new WithdrawPage(Driver, rootSettings, accountId);
    }
    public RegisterAdminPage GoToRegisterAdmin()
    {
        var addNewAdminLink = WaitForClickable(AddNewAdminLinkLocator);
        Click(addNewAdminLink);
        return new RegisterAdminPage(Driver, rootSettings);
    }
    public RequestAccountPage GotoRequestAccountPage()
    {
        var reqLink = WaitForClickable(ReqAccountLinkLocator);
        Click(reqLink);
        return new RequestAccountPage(Driver, rootSettings);
    }
    public string GetLatestRequestIdFromNotifications()
    {
        Click(WaitForClickable(NotificationTabLocator));

        Click(WaitForClickable(AccountRequestItemLocator));

        string rawText = WaitForElementExist(FirstNotificationIdLocator)?.Text.Trim() ?? string.Empty;

        return System.Text.RegularExpressions.Regex.Match(rawText, @"\d+").Value;
    }

    private static string GetRouteIdentifier(IWebElement link)
    {
        var identifier = link
            .GetAttribute("href")?
            .TrimEnd('/')
            .Split('/')
            .Last();

        return !string.IsNullOrWhiteSpace(identifier)
            ? identifier
            : throw new InvalidOperationException(
                "The account ID could not be extracted from the dashboard link."
            );
    }
    // sau khi đăng nhập và thực hiện request account
    public bool CheckDashboardDisplayCorrectly()
    {
        try
        {
            return WaitForVisible(HomeLinkLocator) != null &&
                   WaitForVisible(ProfileLinkLocator) != null &&
                   WaitForVisible(ReqAccountLinkLocator) != null &&
                   WaitForVisible(NotificationTabLocator) != null &&
                   WaitForVisible(DepositLinkLocator) != null &&
                   WaitForVisible(WithdrawLinkLocator) != null &&
                   WaitForVisible(TransferLinkLocator) != null &&
                   WaitForVisible(OutGoingBalanceLinkLocator) != null &&
                   WaitForVisible(IncomingBalanceLinkLocator) != null &&
                   WaitForVisible(DepositLogsLinkLocator) != null &&
                   WaitForVisible(WithdrawLogsLinkLocator) != null &&
                   WaitForVisible(SettingLinkLocator) != null &&
                   WaitForVisible(LogoutLinkLocator) != null &&
                   WaitForVisible(WelcomeMessageLocator) != null;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    public bool CheckDashboardDisplayForNewUser()
    {
        try
        {
            return WaitForVisible(HomeLinkLocator) != null &&
                   WaitForVisible(ProfileLinkLocator) != null &&
                   WaitForVisible(ReqAccountLinkLocator) != null &&
                   WaitForVisible(NotificationTabLocator) != null &&
                   WaitForVisible(SettingLinkLocator) != null &&
                   WaitForVisible(LogoutLinkLocator) != null &&
                   WaitForVisible(WelcomeMessageLocator) != null;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

}
