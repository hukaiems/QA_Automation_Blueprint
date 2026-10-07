using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class WithdrawPage : BasePage
{
    public string AccountId { get; }

    private static readonly By WithdrawAmountInputLocator =
        By.Id("withdrawAmount");

    private static readonly By AccountPasswordInputLocator =
        By.Id("password");

    private static readonly By WalletNumberInputLocator =
        By.Id("wallet-number");

    private static readonly By PinInputLocator =
        By.Id("pin");

    private static readonly By OtpInputLocator =
        By.Id("otp");

    private static readonly By CreditCardPaymentMethodLocator =
        By.Id("credit-card");

    private static readonly By CardNumberInputLocator =
        By.Id("card-number");

    private static readonly By NameOnCardInputLocator =
        By.Id("name-on-card");

    private static readonly By ExpirationDateInputLocator =
        By.Id("expiration-date");

    private static readonly By CvcInputLocator =
        By.Id("cvc");

    private static readonly By WithdrawButtonLocator =
        By.XPath(
            "//button[@type='submit'" +
            " and contains(normalize-space(), 'Withdraw')]"
        );

    private static readonly By SuccessMessageLocator =
        By.XPath(
            "//*[contains(normalize-space(.), 'You Have Withdrawed')" +
            " and contains(normalize-space(.), 'Successfully!')" +
            " and not(.//*[contains(normalize-space(.), 'You Have Withdrawed')" +
            " and contains(normalize-space(.), 'Successfully!')])]"
        );

    public WithdrawPage(
        IWebDriver driver,
        RootSettings settings,
        string accountId)
        : base(driver, settings)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        AccountId = accountId;
        PageUrl = BuildUrl(Routes.Withdraw(accountId));
    }

    public void EnterWithdrawAmount(string amount)
    {
        TypeText(
            WaitForElementExist(WithdrawAmountInputLocator),
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

    public void ClickWithdrawButton()
    {
        var withdrawButton =
            WaitForClickable(WithdrawButtonLocator);

        ScrollToCenter(withdrawButton);
        withdrawButton.Click();
    }

    public void WithdrawByVodafoneCash(
        string amount,
        string accountPassword,
        string walletNumber,
        string pin,
        string otp)
    {
        EnterWithdrawAmount(amount);
        EnterAccountPassword(accountPassword);
        EnterVodafoneCashDetails(walletNumber, pin, otp);
        ClickWithdrawButton();
    }

    public void WithdrawByCreditCard(
        string amount,
        string accountPassword,
        string cardNumber,
        string nameOnCard,
        string expirationDate,
        string cvc)
    {
        EnterWithdrawAmount(amount);
        EnterAccountPassword(accountPassword);
        SelectCreditCardPaymentMethod();

        EnterCreditCardDetails(
            cardNumber,
            nameOnCard,
            expirationDate,
            cvc
        );

        ClickWithdrawButton();
    }

    public string GetSuccessMessage()
    {
        return WaitForVisible(SuccessMessageLocator)
            .Text
            .Trim();
    }
}
