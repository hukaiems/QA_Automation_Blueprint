using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Models.UIData;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class RegisterAdminPage : BasePage
{
    private static readonly By AdminNameInputLocator = By.Id("name");
    private static readonly By AdminPassInputLocator = By.Id("password");
    private static readonly By AdminEmailInputLocator = By.Id("email");
    private static readonly By AdminRepeatPassInputLocator = By.Id("repeatedPassword");
    private static readonly By RegisterBtnLocator = By.XPath("//button[contains(., 'Add Admin')]");
    private static readonly By LogoutLinkLocator = By.XPath("//button[.//span[text()='Logout']]");

    public RegisterAdminPage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {

    }

    public RegisterAdminPage CreateNewAdmin(RegisterAdminForm request)
    {
        var adminNameInput = WaitForElementExist(AdminNameInputLocator);
        var adminPassInput = WaitForElementExist(AdminPassInputLocator);
        var adminEmailInput = WaitForElementExist(AdminEmailInputLocator);
        var adminRepeatPassInput = WaitForElementExist(AdminRepeatPassInputLocator);
        var registerBtn = WaitForClickable(RegisterBtnLocator);
        TypeTextSafe(adminNameInput, request.Name);
        TypeTextSafe(adminEmailInput, request.Email);
        TypeTextSafe(adminPassInput, request.Password);
        TypeTextSafe(adminRepeatPassInput, request.RepeatPassword);
        JavaScriptClick(registerBtn);
        return this;
    }
    public HomePage Logout()
    {
        var logoutLink = WaitForClickable(LogoutLinkLocator);
        JavaScriptClick(logoutLink);
        return new HomePage(Driver, rootSettings);
    }
}
