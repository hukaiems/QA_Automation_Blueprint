using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;
using Team2QA.Automation.Tests.Models.UIData;

namespace Team2QA.Automation.Tests.Pages;

public class RegisterPage : BasePage
{
    private static readonly By FirstNameInputLocator = By.Name("first_name");
    private static readonly By LastNameInputLocator = By.Name("last_name");
    private static readonly By EmailInputLocator = By.Name("email");
    private static readonly By FullAddressInputLocator = By.Name("address");
    private static readonly By PasswordInputLocator = By.Name("password");
    private static readonly By ConfirmPasswordInputLocator = By.Name("repeat_password");
    private static readonly By PhoneNumberInputLocator = By.Name("phone");
    private static readonly By PostalCodeInputLocator = By.Name("postal");
    private static readonly By RegisterBtn = By.XPath("//button[contains(., 'Register')]");
    private static readonly By AlertMessage = By.XPath("//span[text()='Error!']/following-sibling::span");
    public RegisterPage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.Register);
    }
    private void SubmitRegisterForm(RegisterForm data)
    {
        var firstNameInput = WaitForElementExist(FirstNameInputLocator);
        var lastNameInput = WaitForElementExist(LastNameInputLocator);
        var emailInput = WaitForElementExist(EmailInputLocator);
        var fullAddressInput = WaitForElementExist(FullAddressInputLocator);
        var passwordInput = WaitForElementExist(PasswordInputLocator);
        var confirmPasswordInput = WaitForElementExist(ConfirmPasswordInputLocator);
        var phoneNumberInput = WaitForElementExist(PhoneNumberInputLocator);
        var postalCodeInput = WaitForElementExist(PostalCodeInputLocator);
        var registerBtn = WaitForClickable(RegisterBtn);
        TypeTextSafe(firstNameInput, data.FirstName);
        TypeTextSafe(lastNameInput, data.LastName);
        TypeTextSafe(emailInput, data.Email);
        TypeTextSafe(fullAddressInput, data.FullAddress);
        TypeTextSafe(passwordInput, data.Password);
        TypeTextSafe(confirmPasswordInput, data.ConfirmPassword);
        TypeTextSafe(phoneNumberInput, data.PhoneNumber);
        TypeTextSafe(postalCodeInput, data.PostalCode);
        JavaScriptClick(registerBtn);
    }
    public string SubmitRegisterFormErrorValidation(Dictionary<string, string> data)
    {
        var registerForm = new RegisterForm
        {
            FirstName = data.GetValueOrDefault("name", ""),
            LastName = data.GetValueOrDefault("name", ""),
            Email = data.GetValueOrDefault("email", ""),
            FullAddress = data.GetValueOrDefault("addresse", ""),
            Password = data.GetValueOrDefault("password", ""),
            ConfirmPassword = data.GetValueOrDefault("password", ""),
            PhoneNumber = data.GetValueOrDefault("phone", ""),
            PostalCode = data.GetValueOrDefault("postal", "")
        };
        SubmitRegisterForm(registerForm);

        try
        {
            var errorElement = WaitForElementExist(AlertMessage);
            return errorElement?.Text ?? string.Empty;
        }
        catch (WebDriverTimeoutException)
        {
            var fields = new[] { FirstNameInputLocator, EmailInputLocator, FullAddressInputLocator, PasswordInputLocator, PhoneNumberInputLocator, PostalCodeInputLocator };
            foreach (var field in fields)
            {
                var ele = WaitForElementExist(field);
                if (ele != null)
                {
                    var msg = GetHtml5ValidationMessage(ele);
                    if (!string.IsNullOrWhiteSpace(msg))
                    {
                        return msg;
                    }
                }
            }
            return string.Empty;
        }
    }
    public DashboardPage GoToDashBoard(RegisterForm data)
    {
        SubmitRegisterForm(data);
        return new DashboardPage(Driver, rootSettings);
    }

}
