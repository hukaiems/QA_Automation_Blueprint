using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class AdminDashboardPage : BasePage
{
    private static readonly By UsersAccountsRequestTab = By.XPath("(//span[normalize-space()='Users Accounts Request'])[1]");
    private IWebElement AdminLogoutButton => Driver.FindElement(By.XPath("//button[.//span[text()='Logout']]"));
    public AdminDashboardPage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.Dashboard);
    }
    public UsersAccountsRequestPage GoToUsersAccountsRequestPage()
    {
        Click(WaitForClickable(UsersAccountsRequestTab));
        Thread.Sleep(1000);

        return new UsersAccountsRequestPage(Driver, rootSettings);
    }
    public void Logout()
    {
        Click(AdminLogoutButton);
        Thread.Sleep(1000);
    }

}
