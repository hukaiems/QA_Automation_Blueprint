using System.Text.Json.Serialization;

namespace Team2QA.Automation.Tests.Models.ApiContracts;

public sealed record RequestAccountRequest(
    [property: JsonPropertyName("balance")] decimal Balance,
    [property: JsonPropertyName("id")] string UserId,
    [property: JsonPropertyName("oldPassword")] string AccountPassword,
    [property: JsonPropertyName("token")] string CustomerToken
);
