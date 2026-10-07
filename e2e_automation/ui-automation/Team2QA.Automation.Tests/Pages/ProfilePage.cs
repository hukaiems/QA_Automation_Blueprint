using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Models.UIData;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class ProfilePage : BasePage
{
    public ProfilePage(IWebDriver driver, RootSettings settings, string userId) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.UserProfile(userId));
    }
    private static readonly By StatusLocator = By.XPath("//span[text()='Status']/following-sibling::span");
    private static readonly By MemberSinceLocator = By.XPath("//span[text()='Member since']/following-sibling::span");
    private static readonly By PhoneNumberLocator = By.XPath("//div[text()='Phone Number']/following-sibling::div");
    private static readonly By FirstNameLocator = By.XPath("//div[text()='First Name']/following-sibling::div");
    private static readonly By LastNameLocator = By.XPath("//div[text()='Last Name']/following-sibling::div");
    private static readonly By AddressLocator = By.XPath("//div[text()='Address']/following-sibling::div");
    private static readonly By EmailLocator = By.XPath("//div[text()='Email']/following-sibling::div");

    public AccountProfile GetAccountProfile()
    {
        var FirstNameElement = WaitForElementExist(FirstNameLocator);
        var LastNameElement = WaitForElementExist(LastNameLocator);
        var AddressElement = WaitForElementExist(AddressLocator);
        var PhoneNumberElement = WaitForElementExist(PhoneNumberLocator);
        var EmailElement = WaitForElementExist(EmailLocator);
        var StatusElement = WaitForElementExist(StatusLocator);
        var MemberSinceElement = WaitForElementExist(MemberSinceLocator);
        return new AccountProfile(
                FirstName: FirstNameElement?.Text.Trim() ?? string.Empty,
                LastName: LastNameElement?.Text.Trim() ?? string.Empty,
                Address: AddressElement?.Text.Trim() ?? string.Empty,
                PhoneNumber: PhoneNumberElement?.Text.Trim() ?? string.Empty,
                Email: EmailElement?.Text.Trim() ?? string.Empty,
                Status: StatusElement?.Text.Trim() ?? string.Empty,
                MemberSince: MemberSinceElement?.Text.Trim() ?? string.Empty
            );
    }
    public void GotoDashboard()
    {

    }
}
