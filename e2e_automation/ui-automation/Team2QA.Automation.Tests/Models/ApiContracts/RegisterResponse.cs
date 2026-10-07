using Newtonsoft.Json;

namespace Team2QA.Automation.Tests.Models.ApiContracts;

public class RegisterResponse
{
    [JsonProperty("id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;
}

public sealed class RegisterAdminResponse
{
    [JsonProperty("role")]
    public string Role { get; set; } = string.Empty;

    [JsonProperty("_id")]
    public string Id { get; set; } = string.Empty;

    [JsonProperty("admin_name")]
    public string AdminName { get; set; } = string.Empty;

    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}
