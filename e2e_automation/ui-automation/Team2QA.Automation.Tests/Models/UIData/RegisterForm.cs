using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Models.UIData;

// các trường cần truyền vào để tạo user mới
public record RegisterForm
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string FullAddress { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string ConfirmPassword { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string PostalCode { get; init; } = string.Empty;

    public static RegisterForm GenerateRandom()
    {
        string name = StringUtility.GetRandomName();
        string password = "Password123!";
        return new RegisterForm
        {
            FirstName = name,
            LastName = "Team2User999",
            Email = $"{name.ToLower()}@example.com",
            FullAddress = "123 Random St",
            Password = password,
            ConfirmPassword = password,
            PhoneNumber = "01012345678",
            PostalCode = "12345"
        };
    }

}
public record RegisterAdminForm
{
    public string Email { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
    public string RepeatPassword { get; init; } = string.Empty;
    public static RegisterAdminForm GenerateRandom()
    {
        string name = StringUtility.GetRandomName();
        string password = "AdminPassword12345!";
        return new RegisterAdminForm
        {
            Name = name,
            Email = $"Admin{name.ToLower()}@example.com",
            Password = password,
            RepeatPassword = password,
        };
    }
}
