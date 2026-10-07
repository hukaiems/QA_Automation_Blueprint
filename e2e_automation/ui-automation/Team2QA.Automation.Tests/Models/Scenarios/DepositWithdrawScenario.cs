namespace Team2QA.Automation.Tests.Models.Scenarios;

public sealed class DepositWithdrawScenario
{
    public string UserId { get; init; } = string.Empty;
    public string CustomerEmail { get; init; } = string.Empty;
    public string CustomerPassword { get; init; } = string.Empty;
    public string AccountPassword { get; init; } = string.Empty;
    public string CustomerToken { get; init; } = string.Empty;
    public string AccountRequestId { get; init; } = string.Empty;
    public string AccountId { get; set; } = string.Empty;
    public decimal InitialBalance { get; init; }
}
