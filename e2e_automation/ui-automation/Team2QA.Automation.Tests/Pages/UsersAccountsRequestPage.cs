using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Pages;

public class UsersAccountsRequestPage : BasePage
{
    // --- LOCATORS ---
    private static readonly By SearchInputLocator = By.XPath("(//input[@placeholder='search Request'])[1]");
    private static readonly By ApproveBtnLocator = By.XPath("//span[normalize-space()='Approve']");
    private static readonly By DeclineBtnLocator = By.XPath("//button[@type='submit' and .//span[text()='Decline']]");
    private static readonly By AdminAlertLocator = By.XPath("//div[contains(@class, 'border-l-4') and contains(@class, 'shadow-md')]");
    private static readonly By AdminSuccessAlertLocator = By.XPath("//div[contains(., 'Approved Successfully') or contains(@class, 'green')]");

    public UsersAccountsRequestPage(IWebDriver driver, RootSettings settings) : base(driver, settings)
    {
        // Thường trang này load dạng Component/Tab, nếu có URL riêng bạn có thể cấu hình ở đây
    }

    // --- ACTIONS ---
    public void SearchRequestById(string requestId)
    {
        var searchInput = WaitForElementExist(SearchInputLocator);
        searchInput?.Clear();
        TypeText(searchInput, requestId);
        searchInput?.SendKeys(Keys.Enter);

        // Bạn có thể cân nhắc thay thế Thread.Sleep bằng một hàm Wait dữ liệu bảng load xong nếu có
        Thread.Sleep(2000);
    }

    public void ApproveRequest()
    {
        var approveBtn = WaitForClickable(ApproveBtnLocator);
        Thread.Sleep(2000); // Đợi nút ổn định trước khi click
        Click(approveBtn);
    }

    public void DeclineRequest()
    {
        var declineBtn = WaitForClickable(DeclineBtnLocator);
        Click(declineBtn);
    }

    public string GetApprovalSuccessMessage()
    {
        var alertElement = WaitForElementExist(AdminSuccessAlertLocator) ?? throw new NoSuchElementException($"Search input field with locator '{SearchInputLocator}' was not found.");
        return alertElement.Text.Trim();
    }

    public string GetAdminAlertMessage()
    {
        var alertElement = WaitForElementExist(AdminAlertLocator) ?? throw new NoSuchElementException($"Search input field with locator '{AdminAlertLocator}' was not found.");
        return alertElement.Text.Trim();
    }
}
