namespace Team2QA.Automation.Tests.Utils;

public static class ApiEndpoints
{
    // Keep your API paths completely isolated here
    public const string UserRegister = "/api/users";
    public const string UserLogin = "/api/users/login";
    public const string RequestAccount = "/api/request/create";
    public const string AdminRegister = "/api/admins";
    public const string AdminLogin = "/api/admins/login";
    public const string ApproveRequest = "/api/account/create";
    public static string DeleteAdmin(string userId) => $"/api/admins/{userId}";
    public static string DeleteUser(string userId) => $"/api/users/{userId}";
    public static string AccountInfo(string userId) => $"/api/account/{userId}";

    // Example of a dynamic API endpoint for checking balance
    // public static string GetUserBalance(string userId) => $"/api/v1/users/{userId}/balance";
}
