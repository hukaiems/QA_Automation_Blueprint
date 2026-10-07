using AventStack.ExtentReports.Model;
using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.UIData;
using Team2QA.Automation.Tests.Pages;
using Team2QA.Automation.Tests.Utils;
namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class RegisterLoginTest : BaseTest
{
    private static IEnumerable<TestCaseData> RegisterLoginData(string sheetName)
    {
        var allData = ExcelUtil.GetTestData("RegisterLogin.xlsx", sheetName.Trim());
        foreach (var row in allData)
        {
            if (!row.TryGetValue("testCase", out string? testName) || string.IsNullOrWhiteSpace(testName))
            {
                continue;
            }
            var testCase = new TestCaseData(row).SetName($"{row["ID"]}_{testName}");
            yield return testCase;
        }
    }
    [Test]
    [Category("Regression")]
    public async Task IntegrationTest01_RegisterLoginAsAdmin()
    {
        var registerAdmin = RegisterAdminForm.GenerateRandom();
        var login = new LoginPage(Driver, Settings, isAdmin: true);
        login.NavigateTo();
        login.GotoDashBoardAsOwner()
                .GoToRegisterAdmin()
                .CreateNewAdmin(registerAdmin)
                .Logout();
        var newLogin = new LoginPage(Driver, Settings, isAdmin: true);
        newLogin.NavigateTo();
        var adminDashboard = newLogin.GoToDashBoard(registerAdmin.Email,
                                registerAdmin.Password);
        var row_count = adminDashboard.GetTableCount();
        Assert.That(row_count, Is.GreaterThan(0), "Admin Login, User table should appear");
        var helper = new ApiHelper();
        await helper.DeleteAdminAsync(registerAdmin.Email, registerAdmin.Password);
    }

    [Test]
    [Category("Regression")]
    public async Task IntegrationTest02_RegisterLoginAsUser()
    {
        var request = RegisterForm.GenerateRandom();
        var home = new HomePage(Driver, Settings);
        var register = home.GoToRegisterPage();
        register.GoToDashBoard(request)
                .Logout();
        var login = new LoginPage(Driver, Settings);
        login.NavigateTo();
        var dashboard = login.GoToDashBoard(request.Email, request.Password);
        Assert.That(dashboard.CheckDashboardDisplayForNewUser(), Is.True, "Thanh Navbar hiển thị không đúng nội dung cho user mới");
        var profile = dashboard.GoToProfile()
                               .GetAccountProfile();
        using (Assert.EnterMultipleScope())
        {
            Assert.That(profile.PhoneNumber, Is.EqualTo(request.PhoneNumber), "Phone number should match the registered value.");
            Assert.That(profile.Address, Is.EqualTo(request.FullAddress), "Address should match the registered value.");
            Assert.That(profile.Email, Is.EqualTo(request.Email), "Email should match the registered value.");
            Assert.That(profile.FirstName, Is.EqualTo(request.FirstName), "First name should match the registered value.");
            Assert.That(profile.LastName, Is.EqualTo(request.LastName), "Last name should match the registered value.");
        }
        var helper = new ApiHelper();
        await helper.DeleteCustomerAsync(request.Email, request.Password);
    }
    // // //  Tách ra từng sheet cho các negative tests
    [Test]
    [Category("Regression")]
    [TestCaseSource(nameof(RegisterLoginData), new object[] { "RegisterNegative" })]
    public void RegisterNegativeFieldValidation(Dictionary<string, string> data)
    {
        var home = new HomePage(Driver, Settings);
        var registerPage = home.GoToRegisterPage();
        string actualErrorMessage = registerPage.SubmitRegisterFormErrorValidation(data);
        string expectedErrorMessage = data["expectedMessage"];

        AssertErrorMessage(actualErrorMessage, expectedErrorMessage);
    }
    [Test]
    [Category("Regression")]
    [TestCaseSource(nameof(RegisterLoginData), new object[] { "LoginNegative" })]
    public async Task LoginNegativeFieldValidation(Dictionary<string, string> data)
    {
        var isAdmin = bool.Parse(data["isAdmin"]);
        var expectedMessage = data["expectedMessage"];
        var loginPage = new LoginPage(Driver, Settings, isAdmin);
        loginPage.NavigateTo();
        var actualMessage = loginPage.SubmitLoginFormErrorValidation(data["username"], data["password"]);
        AssertErrorMessage(actualMessage, expectedMessage);
    }
    [Test]
    [Category("Regression")]
    [TestCaseSource(nameof(RegisterLoginData), new object[] { "AccessControl" })]
    public async Task LoginNegativeAccessControl(Dictionary<string, string> data)
    {
        bool isAdmin = bool.Parse(data["isAdmin"]);
        string testCase = data["testCase"];
        string expectedMessage = data["expectedMessage"];
        bool isDeleted = bool.Parse(data["isDeleted"]);

        string email,password;
        var helper = new ApiHelper();
        if (isAdmin)
        {
            var registerAdmin = RegisterAdminForm.GenerateRandom();
            email = registerAdmin.Email;
            password = registerAdmin.Password;
            await helper.RegisterAdminFromFormAsync(registerAdmin);
            
            if (isDeleted)
            {
                await helper.DeleteAdminAsync(email, password);
            }
        }
        else
        {
            var request = RegisterForm.GenerateRandom();
            email = request.Email;
            password = request.Password;
            await helper.RegisterUserFromFormAsync(request);

            if (isDeleted)
            {
                await helper.DeleteCustomerAsync(email, password);
            }
        }

        bool targetIsAdminPage = false;
        if (testCase == "User Account Cannot Login on Admin Page" || testCase == "Deleted Admin Account Cannot Login")
        {
            targetIsAdminPage = true;
        }

        var loginPage = new LoginPage(Driver, Settings, targetIsAdminPage);
        loginPage.NavigateTo();
        var actualMessage = loginPage.SubmitLoginFormErrorValidation(email, password);

        AssertErrorMessage(actualMessage, expectedMessage);
    }

    private static void AssertErrorMessage(string actualMessage, string expectedMessage)
    {
        if (string.IsNullOrEmpty(expectedMessage))
        {
            return;
        }

        var expectedMessages = expectedMessage.Split('|', StringSplitOptions.RemoveEmptyEntries).Select(m => m.Trim());
        bool isMatch = expectedMessages.Any(actualMessage.Contains);

        Assert.That(isMatch, Is.True, 
            $"Thất bại : Hiển thị không đúng lỗi. Actual: '{actualMessage}', Expected one of: '{expectedMessage}'");
    }

}
