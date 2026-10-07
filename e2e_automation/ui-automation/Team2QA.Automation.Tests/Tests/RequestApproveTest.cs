using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Pages;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
[Ignore("Skipping this test script")]
public class RequestApproveTest : BaseTest
{
    private static IEnumerable<TestCaseData> RequestAndApproveData()
    {
        var allData = ExcelUtil.GetTestData("RequestAndApprove.xlsx", "TestCase");

        foreach (var row in allData)
        {
            if (!row.ContainsKey("Scenario") || string.IsNullOrWhiteSpace(row["Scenario"]))
            {
                continue;
            }
            string testName = row["Scenario"].Trim();
            var testCase = new TestCaseData(row).SetName(testName);

            yield return testCase;
        }
    }
    [Test]
    [Category("Regression")]

    [TestCaseSource(nameof(RequestAndApproveData))]
    public async Task TC03_RequestAndApproveFlow(Dictionary<string, string> data)
    {
        // --- 1. TỰ ĐỘNG ĐĂNG KÝ USER MỚI QUA API (ÁP DỤNG CHO TẤT CẢ CÁC CASE) ---
        bool isMaxLimitCase = data["Scenario"].Contains("MAX_ACCOUNT_LIMIT");

        if (!isMaxLimitCase)
        {
            using var apiClient = new ApiClient();
            var registerApi = new RegisterApiClient(apiClient);
            string uniqueId = Guid.NewGuid().ToString().Substring(0, 8);
            string randomEmail = $"testuser_{uniqueId}@gmail.com";
            string randomName = $"Automation User {uniqueId}";
            string fixedPhone = "01008878980";

            Console.WriteLine($"[API_SETUP] Đang tự động đăng ký tài khoản mới qua API cho scenario [{data["Scenario"]}]: {randomEmail}");

            await registerApi.RegisterCustomerAsync(new RegisterCustomerRequest(
                randomName, randomEmail, data["UserPassword"], "District 6, HCMC", fixedPhone, "70000"
            ));

            data["UserEmail"] = randomEmail;
            Console.WriteLine($"[API_SETUP] Đăng ký thành công! Tài khoản testcase này sẽ dùng: {randomEmail}");
        }
        else
        {
            Console.WriteLine($"[EXCEL_DATA] Phát hiện scenario MAX_ACCOUNT_LIMIT. Sử dụng tài khoản có sẵn từ file Excel: {data["UserEmail"]}");
        }

        // --- GIAI ĐOẠN 1: CUSTOMER ĐĂNG NHẬP & GỬI REQUEST ---
        var home = new HomePage(Driver, Settings);
        var loginPage = home.GotoLoginPage();

        var customerDashboard = loginPage.GoToDashBoard(data["UserEmail"], data["UserPassword"]);
        var requestAccountPage = customerDashboard.GotoRequestAccountPage();

        if (data["Scenario"].Contains("EMPTY") || data["Scenario"].Contains("ZERO_BALANCE") || data["Scenario"].Contains("NEGATIVE_BALANCE"))
        {
            requestAccountPage.SubmitAccountRequest(data);

            string actualValidationMsg = string.Empty;

            if (data["Scenario"].Contains("EMPTY"))
            {
                actualValidationMsg = requestAccountPage.GetEmptyFieldValidationMessage(data["Scenario"]) ?? string.Empty;
            }
            else
            {
                actualValidationMsg = requestAccountPage.GetInitialBalanceValidationMessage() ?? string.Empty;
            }

            string expectedValidationMsg = data.ContainsKey("ExpectedMessage") && !string.IsNullOrEmpty(data["ExpectedMessage"])
                ? data["ExpectedMessage"]
                : "Please fill out this field."; 

            bool isChromeMsg = actualValidationMsg.Contains(expectedValidationMsg);
            bool isFirefoxMsg = actualValidationMsg.Contains("Please fill in this field") || actualValidationMsg.Contains("500");

            Assert.That(isChromeMsg || isFirefoxMsg, Is.True,
                $"Thất bại [{data["Scenario"]}]: Trình duyệt không hiển thị đúng thông báo Native validation! (Actual: '{actualValidationMsg}')");

            Console.WriteLine($"[✓ PASSED] {data["Scenario"]} -> Bắt thành công HTML5 Native Validation: '{actualValidationMsg}'");

            customerDashboard.Logout();
            return;
        }


        if (data["Scenario"].Contains("WRONG_PASSWORD"))
        {
            requestAccountPage.SubmitAccountRequest(data);

            string actualErrorMessage = requestAccountPage.GetAlertMessage();
            string expectedErrorMessage = data.ContainsKey("ExpectedMessage") && !string.IsNullOrEmpty(data["ExpectedMessage"])
                ? data["ExpectedMessage"]
                : "Error!Wrong old password";

            Assert.That(actualErrorMessage, Contains.Substring(expectedErrorMessage),
                $"Thất bại [{data["Scenario"]}]: Hệ thống không chặn hoặc hiển thị sai thông báo lỗi sai mật khẩu!");

            Console.WriteLine($"[✓ PASSED] {data["Scenario"]} -> Bắt thành công System Alert: {actualErrorMessage}");

            customerDashboard.Logout();
            return;
        }

        if (data["Scenario"].Contains("DUPLICATE_REQUEST_ERROR"))
        {

            requestAccountPage.SubmitAccountRequest(data);

            string customerAlertMsg = requestAccountPage.GetSuccessMessage();
            Assert.That(customerAlertMsg, Contains.Substring("Your Account Request Has Been Sent Successfully!"),
                "Thất bại: Giao diện Customer không hiển thị đúng thông báo gửi Request thành công!");

            requestAccountPage.ClickSubmitButtonOnly();

            string actualErrorMessage = requestAccountPage.GetAlertMessage();
            string expectedErrorMessage = data.ContainsKey("ExpectedMessage")
                ? data["ExpectedMessage"]
                : "Error!Sorry, you Already Has Sent An Account Request!, Please Wait For Our Response Soon.";

            Assert.That(actualErrorMessage, Contains.Substring(expectedErrorMessage),
                "Thất bại: Hệ thống không chặn gửi trùng hoặc hiển thị sai thông báo lỗi!");

            Console.WriteLine($"[DUPLICATE CASE] {data["Scenario"]} -> System block request when there is a pending one");
            customerDashboard.Logout();
            return;
        }
        else
        {
            requestAccountPage.SubmitAccountRequest(data);

            string customerAlertMsg = requestAccountPage.GetSuccessMessage();
            Assert.That(customerAlertMsg, Contains.Substring("Your Account Request Has Been Sent Successfully!"),
                "Thất bại: Giao diện Customer không hiển thị đúng thông báo gửi Request thành công!");

            string liveRequestId = customerDashboard.GetLatestRequestIdFromNotifications();

            Console.WriteLine($"[START CASE] {data["Scenario"]} -> Khởi tạo Request thành công. Mã ID hệ thống cấp: {liveRequestId}");
            customerDashboard.Logout();

            // --- GIAI ĐOẠN 2: ADMIN ĐĂNG NHẬP & PHÊ DUYỆT ---
            var adminLogin = new LoginPage(Driver, Settings, true);
            adminLogin.NavigateTo();
            var adminDashboard = adminLogin.GoToAdminDashBoard(data["AdminEmail"], data["AdminPassword"]);

            var requestListPage = adminDashboard.GoToUsersAccountsRequestPage();
            requestListPage.SearchRequestById(liveRequestId);

            if (data["Scenario"].Contains("DECLINE"))
            {
                requestListPage.DeclineRequest();
                string adminAlertMsg = requestListPage.GetAdminAlertMessage();
                string expectedDeclineMsg = data.ContainsKey("ExpectedMessage") ? data["ExpectedMessage"] : "Request Declined Successfully!";

                Assert.That(adminAlertMsg, Contains.Substring(expectedDeclineMsg),
                    "Thất bại: Hệ thống không hiển thị đúng thông báo Admin đã TỪ CHỐI thành công!");
                Console.WriteLine($"[✓ PASSED] {data["Scenario"]} -> Admin đã Từ Chối (Decline) thành công Request ID: {liveRequestId}.\n");
            }
            else if (isMaxLimitCase)
            {
                Console.WriteLine($"[MAX_LIMIT] Tiến hành Approve Request ID: {liveRequestId} (đã có sẵn 3 accounts trước đó)...");
                requestListPage.ApproveRequest();

                string adminAlertMsg = requestListPage.GetAdminAlertMessage();
                string expectedLimitMsg = data.ContainsKey("ExpectedMessage")
                    ? data["ExpectedMessage"]
                    : "Error!User validation failed: no_of_account: Sorry, You Can Not Add More Than 3 Accounts in your Bank Profile";

                Assert.That(adminAlertMsg, Contains.Substring(expectedLimitMsg),
                    "Thất bại: Hệ thống không chặn Admin duyệt hoặc hiển thị sai thông báo lỗi giới hạn 3 tài khoản!");
                Console.WriteLine($"[✓ PASSED] Xuất hiện thông báo lỗi mong muốn: {adminAlertMsg}");


                Console.WriteLine($"[MAX_LIMIT] Tiến hành Decline Request ID: {liveRequestId} sau khi bị chặn Approve...");
                requestListPage.DeclineRequest();

                string declineSuccessMsg = requestListPage.GetAdminAlertMessage();
                Assert.That(declineSuccessMsg, Contains.Substring("Request Declined Successfully!"),
                    "Thất bại: Admin không thể từ chối (Decline) request sau khi bị chặn Approve!");
                Console.WriteLine($"[✓ PASSED] {data["Scenario"]} -> Đã chặn Approve thành công & Decline dọn dẹp Request ID: {liveRequestId} hoàn tất.\n");
            }
            else
            {
                requestListPage.ApproveRequest();
                string adminAlertMsg = requestListPage.GetApprovalSuccessMessage();
                Assert.That(adminAlertMsg, Contains.Substring("Request Approved Successfully!"),
                    "Thất bại: Hệ thống không hiển thị thông báo Admin đã phê duyệt thành công!");
                Console.WriteLine($"[✓ PASSED] {data["Scenario"]} -> Admin đã Phê Duyệt (Approve) thành công Request ID: {liveRequestId}.\n");

            }
        }
    }

}
