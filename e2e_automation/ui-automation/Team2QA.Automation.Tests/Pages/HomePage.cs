using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;
namespace Team2QA.Automation.Tests.Pages;

public class HomePage : BasePage
{
    private static readonly By RegisterBtn = By.XPath("//div[contains(@class,'lg:flex')]//a[@href='/register']");
    private static readonly By LoginBtn = By.XPath("//div[contains(@class,'lg:flex')]//a[@href='/login']");
    private static readonly By ReqAccountBtn = By.PartialLinkText("Request Account");
    public HomePage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.Home);
        NavigateTo();
    }
    public RegisterPage GoToRegisterPage()
    {
        var registerBtn = WaitForClickable(RegisterBtn);
        Click(registerBtn);
        return new RegisterPage(Driver, rootSettings);
    }
    public LoginPage GotoLoginPage()
    {
        var loginBtn = WaitForClickable(LoginBtn);
        Click(loginBtn);
        return new LoginPage(Driver, rootSettings, false);
        // Fluent Page Object Pattern/ Fluent Interface (FPOP)
    }
    public RequestAccountPage GotoRequestAccountPage()
    {
        var reqAccountBtn = WaitForClickable(ReqAccountBtn);
        Click(reqAccountBtn);
        return new RequestAccountPage(Driver, rootSettings);
    }

    public bool IsLoginButtonDisplayed()
    {
        // if dont use try, catch then if element not found, exception will be thrown and test will fail.
        try
        {
            var isLoginBtnDisplayed = WaitForClickable(LoginBtn);
            return isLoginBtnDisplayed.Displayed ? true : false;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }

    }
}
