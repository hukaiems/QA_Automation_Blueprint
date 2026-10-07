using Team2QA.Automation.Tests.Utils;
using Team2QA.Automation.Tests.Base;
using OpenQA.Selenium;

namespace Team2QA.Automation.Tests.Pages;

public class TransferPage : BasePage
{
    private static readonly By TransferHeaderLocator = By.XPath("//h3[normalize-space()='Transfer Money']");
    private static readonly By TransferAmountInputLocator = By.Id("balanceTransfered");
    private static readonly By RecipientInputLocator = By.Id("recipientId");
    private static readonly By PasswordInputLocator = By.Id("password");
    private static readonly By TransferSuccessMessageLocator = By.XPath("//div[contains(@class,'false')]");

    public TransferPage(IWebDriver driver, RootSettings settings, string userId) : base(driver, settings)
    {
        PageUrl = BuildUrl(Routes.Transfer(userId)); // Pass the userId parameter
    }

    public bool IsTransferHeaderDisplayed()
    {
        try
        {
            var transferHeader = WaitForElementExist(TransferHeaderLocator);
            return transferHeader?.Displayed ?? false;
        }
        catch (WebDriverTimeoutException)
        {
            return false;
        }
    }

    // method to type in transfer form
    public void FillTransferForm(string recipientId, string amount, string description)
    {
        var transferAmountInput = WaitForElementExist(TransferAmountInputLocator);
        TypeTextSafe(transferAmountInput, amount);
        var recipientInput = WaitForElementExist(RecipientInputLocator);
        TypeTextSafe(recipientInput, recipientId);
        var passwordInput = WaitForElementExist(PasswordInputLocator);
        TypeTextSafe(passwordInput, description);

        var submitButton = WaitForElementExist(By.CssSelector("button[type='submit']"));
        Click(submitButton);
    }

    public bool IsTransferSuccessMessageDisplayed()
    {
        try
        {
            var successMessage = WaitForVisible(TransferSuccessMessageLocator);

            // Check if element is null just in case
            if (successMessage == null)
            {
                Console.WriteLine("Failure: Element was null.");
                return false;
            }

            string actualText = successMessage.Text;

            if (actualText.Contains("You Have Transfered"))
            {
                return true;
            }
            else
            {
                // The element exists, but the text is wrong
                Console.WriteLine($"Failure: Text mismatch. Actual text was: '{actualText}'");
                return false;
            }
        }
        catch (WebDriverTimeoutException)
        {
            // The element never appeared
            Console.WriteLine("Failure: Element did not appear within the timeout period.");
            return false;
        }
    }
}
