using System.Text.Json.Serialization;

namespace Team2QA.Automation.Tests.Models.ApiContracts;

public sealed record LoginRequest(
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password
);
