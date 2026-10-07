using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class RequestAccountPage : BasePage
{
    private static readonly By InitialBalanceInput = By.XPath("(//input[@id='intitialBalance'])[1]");
    private static readonly By PasswordInput = By.XPath("(//input[@id='password'])[1]");

    // Vodafone Cash Fields
    private static readonly By VodafoneCashInput = By.XPath("(//input[@id='vodafoneCash'])[1]");
    private static readonly By WalletNumberInput = By.XPath("(//input[@id='wallet-number'])[1]");
    private static readonly By PinInput = By.XPath("(//input[@id='pin'])[1]");
    private static readonly By OtpInput = By.XPath("(//input[@id='otp'])[1]");

    // Credit Card Fields
    private static readonly By CreditCardInput = By.XPath("(//input[@id='credit-card'])[1]");
    private static readonly By CardNumberInput = By.XPath("(//input[@id='card-number'])[1]");
    private static readonly By NameOnCardInput = By.XPath("(//input[@id='name-on-card'])[1]");
    private static readonly By ExpirationDateInput = By.XPath("(//input[@id='expiration-date'])[1]");
    private static readonly By CvcInput = By.XPath("(//input[@id='cvc'])[1]");

    // Submit Button
    private static readonly By SubmitBtn = By.XPath("(//button[@type='submit'])[1]");
    private static readonly By SuccessAlertLocator = By.XPath("//div[contains(@class, 'green') or contains(., 'Successfully')]");
    private static readonly By AlertMessageLocator = By.XPath("//div[./div/span[contains(text(), 'Error!') or contains(text(), 'Successfully')]]");

    public RequestAccountPage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.AccountRequest);
    }

    public void SubmitAccountRequest(Dictionary<string, string> data)
    {
        TypeText(WaitForElementExist(InitialBalanceInput), data["InitialBalance"]);
        TypeText(WaitForElementExist(PasswordInput), data["UserPasswordConfirm"]);

        string method = data["PaymentMethod"];

        if (method == "Vodafone")
        {
            var vodafoneRadio = WaitForClickable(VodafoneCashInput);

            Click(vodafoneRadio);

            TypeText(WaitForElementExist(WalletNumberInput), data["WalletNumber"]);
            TypeText(WaitForElementExist(PinInput), data["Pin"]);
            TypeText(WaitForElementExist(OtpInput), data["Otp"]);

        }
        else if (method == "CreditCard")
        {
            var creditCardRadio = WaitForClickable(CreditCardInput);
            Click(creditCardRadio);

            TypeText(WaitForElementExist(CardNumberInput), data["CardNumber"]);
            TypeText(WaitForElementExist(NameOnCardInput), data["NameOnCard"]);
            TypeText(WaitForElementExist(ExpirationDateInput), data["ExpirationDate"]);
            TypeText(WaitForElementExist(CvcInput), data["Cvc"]);
        }
        Click(WaitForClickable(SubmitBtn));
    }
    public void ClickSubmitButtonOnly()
    {
        var submitBtn = WaitForClickable(SubmitBtn);
        JavaScriptClick(submitBtn);
    }
    public string GetSuccessMessage()
    {
        var alertElement = WaitForElementExist(SuccessAlertLocator);
        return alertElement?.Text.Trim() ?? string.Empty;
    }
    public string GetAlertMessage()
    {
        var alertElement = WaitForElementExist(AlertMessageLocator);
        return alertElement?.Text.Trim() ?? string.Empty;
    }
    public string? GetInitialBalanceValidationMessage()
    {
        var ele = WaitForElementExist(InitialBalanceInput);
        return ele != null ? GetHtml5ValidationMessage(ele) : string.Empty;
    }
    public string GetEmptyFieldValidationMessage(string scenarioName)
    {
        IWebElement targetElement = null;

        if (scenarioName.Contains("EMPTY_WALLET_NUMBER"))
            targetElement = Driver.FindElement(By.Id("wallet-number")); // Thay bằng By / Id tương ứng trên UI
        else if (scenarioName.Contains("EMPTY_PIN"))
            targetElement = Driver.FindElement(By.Id("pin"));
        else if (scenarioName.Contains("EMPTY_OTP"))
            targetElement = Driver.FindElement(By.Id("otp"));
        else if (scenarioName.Contains("EMPTY_CARD_NUMBER"))
            targetElement = Driver.FindElement(By.Id("card-number"));
        else if (scenarioName.Contains("EMPTY_NAME_ON_CARD"))
            targetElement = Driver.FindElement(By.Id("name-on-card"));
        else if (scenarioName.Contains("EMPTY_EXPIRATION_DATE"))
            targetElement = Driver.FindElement(By.Id("expiration-date"));
        else if (scenarioName.Contains("EMPTY_CVC"))
            targetElement = Driver.FindElement(By.Id("cvc"));

        if (targetElement != null)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)Driver;
            return (string)js.ExecuteScript("return arguments[0].validationMessage;", targetElement);
        }

        return string.Empty;
    }
}
