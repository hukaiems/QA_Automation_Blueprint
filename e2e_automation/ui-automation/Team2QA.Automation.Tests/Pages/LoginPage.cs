using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class LoginPage : BasePage
{
    private static readonly By EmailInputLocator = By.Name("email");
    private static readonly By PasswordInputLocator = By.Name("password");
    private static readonly By LoginBtnLocator = By.XPath("//button[contains(.,'Login')]");
    private static readonly By ErrorMessageLocator = By.XPath("//span[text()='Error!']/following-sibling::span");

    public LoginPage(IWebDriver driver, RootSettings settings, bool isAdmin = false) : base(driver, settings)
    {
        PageUrl = isAdmin == true ? BuildUrl(Routes.AdminLogin) : BuildUrl(Routes.Login);
    }
    private void SubmitLoginForm(string email, string password)
    {
        var emailInput = WaitForElementExist(EmailInputLocator);
        var passwordInput = WaitForElementExist(PasswordInputLocator);
        TypeText(emailInput, email);
        TypeText(passwordInput, password);
        var loginBtn = WaitForClickable(LoginBtnLocator);
        Click(loginBtn);
    }
    public DashboardPage GoToDashBoard(string email, string password)
    {
        SubmitLoginForm(email, password);
        return new DashboardPage(Driver, rootSettings);
    }
    public DashboardPage GotoDashBoardAsOwner() => GoToDashBoard(rootSettings.Admin.Username, rootSettings.Admin.Password);
    public AdminDashboardPage GoToAdminDashBoard(string email, string password)
    {
        SubmitLoginForm(email, password);
        return new AdminDashboardPage(Driver, rootSettings);
    }
    public string GetFieldValidationMessage(By locator)
    {
        var ele = WaitForElementExist(locator);
        if (ele != null)
        {
            return GetHtml5ValidationMessage(ele) ?? string.Empty;
        }
        return string.Empty;
    }
    public string SubmitLoginFormErrorValidation(string email, string password)
    {
        SubmitLoginForm(email, password);
        var emailMessage = GetFieldValidationMessage(EmailInputLocator);
        var passwordMessage = GetFieldValidationMessage(PasswordInputLocator);
        // invalid or empty email
        if (!string.IsNullOrWhiteSpace(emailMessage))
        {
            return emailMessage;
        }
        // empty password
        if (!string.IsNullOrWhiteSpace(passwordMessage))
        {
            return passwordMessage;
        }
        // invalid password
        var errorElement = WaitForElementExist(ErrorMessageLocator);
        return errorElement?.Text ?? string.Empty;
    }
    public bool IsLoginButtonDisplayed()
    {
        try
        {
            var loginBtn = WaitForClickable(LoginBtnLocator);
            return loginBtn.Displayed;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
}
