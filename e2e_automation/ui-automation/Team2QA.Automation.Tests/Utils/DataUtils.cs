using Team2QA.Automation.Tests.Models.ApiContracts;

namespace Team2QA.Automation.Tests.Utils;

public static class DataUtils
{
    public static string GenerateRandomEmail(string prefix = "user")
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        return $"{prefix}_{timestamp}@fpt.com";
    }

    public static RegisterAdminRequest GenerateRandomRegisterAdminRequest()
    {
        string timestamp = DateTime.Now.ToString("yyyyMMddHHmmssfff");
        return new RegisterAdminRequest(
            Name: $"AutoAdmin_{timestamp}",
            Email: GenerateRandomEmail("admin"),
            Token: $"dummy_token_{timestamp}",
            Role: "admin",
            Password: "P@ssw0rdP@ssw0rd"
        );
    }


}
