namespace Team2QA.Automation.Tests.Models.ApiContracts;

public record AuthResponse
{
    public string Token { get; init; } = string.Empty;
    public string Id { get; init; } = string.Empty;
}
