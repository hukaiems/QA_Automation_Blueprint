namespace Team2QA.Automation.Tests.Utils;

public static class Routes
{
    public const string Home = "/";
    public const string Login = "/login";
    public const string AdminLogin = "/admins/login";
    public const string Register = "/register";
    public const string AdminRegister = "/";
    public const string Dashboard = "/";
    public const string Settings = "/settings";
    public static string UserProfile(string userId) => $"/profile/{userId}";
    public static string Transfer(string userId) => $"/account/transfer/{userId}";
    // public static string Deposit(string accountId) => $"/account/deposit/{accountId}";
    public static string Deposit(string accountId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        return $"/account/deposit/{accountId}";
    }
    public static string Withdraw(string accountId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        return $"/account/withdraw/{accountId}";
    }
    public static string DepositLogs(string accountId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        return $"/account/deposit-logs/{accountId}";
    }
    public static string WithdrawLogs(string accountId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(accountId);

        return $"/account/withdraw-logs/{accountId}";
    }
    public const string AccountRequest = "/";
    public static string AdminProfile(string userId) => $"/admins/profile/{userId}";

    // Ví dụ cho các đường dẫn cần truyền tham số (tham khảo)
    // public static string UserDetail(int userId)  => $"/users/{userId}";
    // public static string EditPost(int postId)    => $"/posts/{postId}/edit";


}
