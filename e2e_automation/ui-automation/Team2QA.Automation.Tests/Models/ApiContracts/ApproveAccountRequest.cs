
using System.Text.Json.Serialization;

namespace Team2QA.Automation.Tests.Models.ApiContracts;

public sealed record ApproveAccountRequest(
    [property: JsonPropertyName("balance")] decimal Balance,
    [property: JsonPropertyName("id")] string UserId,
    [property: JsonPropertyName("request_id")] long RequestId,
    [property: JsonPropertyName("token")] string AdminToken
);
