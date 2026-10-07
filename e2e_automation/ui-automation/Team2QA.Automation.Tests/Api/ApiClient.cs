using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Api;

// API Wrapper -> perform all HTTP lifting
/*
With this Wrapper, every HTTP call (API call) will going through this like a MiddleWare => Proxy in testing
 */
public sealed class ApiClient : IDisposable // so C# know to dispose the socker connect
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public ApiClient(string baseUrl = "")
    {
        if (string.IsNullOrWhiteSpace(baseUrl))
        {
            baseUrl = ConfigReader.Instance.AppSettings.BaseUrl;
        }

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    //Task == Promise
    public async Task<TResponse> PostAsync<TRequest, TResponse>(string endpoint, TRequest payload, string? bearerToken = null)
    {

        using var request = new HttpRequestMessage(HttpMethod.Post, endpoint.TrimStart('/'));

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        var json = JsonSerializer.Serialize(payload);
        request.Content = new StringContent(json, Encoding.UTF8, "application/json");

        using var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"POST {endpoint} failed. Status: {(int)response.StatusCode}. Body: {responseBody}"
            );
        }

        return JsonSerializer.Deserialize<TResponse>(responseBody, _jsonOptions)
            ?? throw new Exception($"Cannot deserialize response from: {endpoint}. Body: {responseBody}");
    }

    public async Task<TResponse> GetAsync<TResponse>(
        string endpoint,
        string? bearerToken = null)
    {
        using var request = new HttpRequestMessage(
            HttpMethod.Get,
            endpoint.TrimStart('/')
        );

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        using var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"GET {endpoint} failed. Status: {(int)response.StatusCode}. Body: {responseBody}"
            );
        }

        return JsonSerializer.Deserialize<TResponse>(responseBody, _jsonOptions)
            ?? throw new Exception(
                $"Cannot deserialize response from: {endpoint}. Body: {responseBody}"
            );
    }

    public async Task DeleteAsync(string endpoint, string bearerToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint.TrimStart('/'));

        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);

        using var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"DELETE {endpoint} failed. Status: {(int)response.StatusCode}. Body: {responseBody}"
            );
        }
    }

    public async Task<TResponse> DeleteAsync<TResponse>(string endpoint, string? bearerToken = null)
    {
        using var request = new HttpRequestMessage(HttpMethod.Delete, endpoint.TrimStart('/'));

        if (!string.IsNullOrWhiteSpace(bearerToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", bearerToken);
        }

        using var response = await _httpClient.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception(
                $"DELETE {endpoint} failed. Status: {(int)response.StatusCode}. Body: {responseBody}"
            );
        }

        return JsonSerializer.Deserialize<TResponse>(responseBody, _jsonOptions)
            ?? throw new Exception(
                $"Cannot deserialize response from: {endpoint}. Body: {responseBody}"
            );
    }

    public void Dispose()
    {
        _httpClient.Dispose();
    }
}
