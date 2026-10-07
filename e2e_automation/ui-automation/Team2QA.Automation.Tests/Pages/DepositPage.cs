using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class DepositPage : BasePage
{
    public string AccountId { get; }

    // Stable IDs found from the Deposit page HTML.
    private readonly By DepositAmountInputLocator =
        By.Id("depositAmount");

    private readonly By AccountPasswordInputLocator =
        By.Id("password");

    private readonly By WalletNumberInputLocator =
        By.Id("wallet-number");

    private readonly By PinInputLocator =
        By.Id("pin");

    private readonly By OtpInputLocator =
        By.Id("otp");

    private readonly By CreditCardPaymentMethodLocator =
        By.Id("credit-card");

    private readonly By CardNumberInputLocator =
        By.Id("card-number");

    private readonly By NameOnCardInputLocator =
        By.Id("name-on-card");

    private readonly By ExpirationDateInputLocator =
        By.Id("expiration-date");

    private readonly By CvcInputLocator =
        By.Id("cvc");

    // The button does not currently have an ID.
    private readonly By DepositButtonLocator =
        By.XPath(
            "//button[@type='submit' and contains(normalize-space(), 'Deposit')]"
        );

    // The success notification only appears after a successful deposit.
    private readonly By SuccessMessageLocator =
        By.XPath(
            "//*[contains(normalize-space(.), 'You Have Deposited')" +
            " and contains(normalize-space(.), 'Successfully!')" +
            " and not(.//*[contains(normalize-space(.), 'You Have Deposited')" +
            " and contains(normalize-space(.), 'Successfully!')])]"
        );

    public DepositPage(
        IWebDriver driver,
        RootSettings settings,
        string accountId)
        : base(driver, settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        AccountId = accountId;
        PageUrl = BuildUrl(Routes.Deposit(accountId));
    }

    public void EnterDepositAmount(string amount)
    {
        TypeText(
            WaitForElementExist(DepositAmountInputLocator),
            amount
        );
    }

    public void EnterAccountPassword(string accountPassword)
    {
        TypeText(
            WaitForElementExist(AccountPasswordInputLocator),
            accountPassword
        );
    }

    public void EnterVodafoneCashDetails(
        string walletNumber,
        string pin,
        string otp)
    {
        TypeText(
            WaitForElementExist(WalletNumberInputLocator),
            walletNumber
        );

        TypeText(
            WaitForElementExist(PinInputLocator),
            pin
        );

        TypeText(
            WaitForElementExist(OtpInputLocator),
            otp
        );
    }

    public void SelectCreditCardPaymentMethod()
    {
        Click(
            WaitForClickable(CreditCardPaymentMethodLocator)
        );
    }

    public void EnterCreditCardDetails(
        string cardNumber,
        string nameOnCard,
        string expirationDate,
        string cvc)
    {
        TypeText(
            WaitForElementExist(CardNumberInputLocator),
            cardNumber
        );

        TypeText(
            WaitForElementExist(NameOnCardInputLocator),
            nameOnCard
        );

        TypeText(
            WaitForElementExist(ExpirationDateInputLocator),
            expirationDate
        );

        TypeText(
            WaitForElementExist(CvcInputLocator),
            cvc
        );
    }

    public void ClickDepositButton()
    {
        Click(
            WaitForClickable(DepositButtonLocator)
        );
    }

    public void DepositByVodafoneCash(
        string amount,
        string accountPassword,
        string walletNumber,
        string pin,
        string otp)
    {
        EnterDepositAmount(amount);
        EnterAccountPassword(accountPassword);

        // Vodafone Cash is selected by default on the current page.
        EnterVodafoneCashDetails(walletNumber, pin, otp);

        ClickDepositButton();
    }

    public void DepositByCreditCard(
        string amount,
        string accountPassword,
        string cardNumber,
        string nameOnCard,
        string expirationDate,
        string cvc)
    {
        EnterDepositAmount(amount);
        EnterAccountPassword(accountPassword);
        SelectCreditCardPaymentMethod();

        EnterCreditCardDetails(
            cardNumber,
            nameOnCard,
            expirationDate,
            cvc
        );

        ClickDepositButton();
    }

    public string GetSuccessMessage()
    {
        return WaitForVisible(SuccessMessageLocator)
            .Text
            .Trim();
    }

    public bool IsDepositFormDisplayed()
    {
        try
        {
            WaitForElementExist(
                DepositAmountInputLocator,
                TimeSpan.FromSeconds(10)
            );

            return true;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }
}
