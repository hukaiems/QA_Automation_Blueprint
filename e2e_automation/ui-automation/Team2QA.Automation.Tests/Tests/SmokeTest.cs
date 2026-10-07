using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.UIData;
using Team2QA.Automation.Tests.Pages;

namespace Team2QA.Automation.Tests.Tests;

[TestFixture]
[Parallelizable(ParallelScope.Children)]
public class SmokeTest : BaseTest
{
    [Test]
    [Category("Smoke")]

    public async Task Should_RegisterAndLoginNormally()
    {
        var request = RegisterForm.GenerateRandom();
        var home = new HomePage(Driver, Settings);
        var register = home.GoToRegisterPage();
        register.GoToDashBoard(request)
                .Logout();
        var login = new LoginPage(Driver, Settings);
        login.NavigateTo();
        var dashboard = login.GoToDashBoard(request.Email, request.Password);
        Assert.That(dashboard.CheckDashboardDisplayForNewUser(), Is.True,
                    "Thanh Navbar hiển thị không đúng nội dung cho user mới");
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
}
