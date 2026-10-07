using System.Text.Json.Serialization;

namespace Team2QA.Automation.Tests.Models.ApiContracts;

public sealed record RegisterAdminRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("token")] string Token,
    [property: JsonPropertyName("role")] string Role,
    [property: JsonPropertyName("password")] string Password
);

public sealed record RegisterCustomerRequest(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("email")] string Email,
    [property: JsonPropertyName("password")] string Password,
    [property: JsonPropertyName("addresse")] string Address,
    [property: JsonPropertyName("phone")] string Phone,
    [property: JsonPropertyName("postal")] string Postal
);

