using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Models.UIData;

namespace Team2QA.Automation.Tests.Api;

public class ApiHelper : IDisposable
{
    private readonly ApiClient _client;
    private readonly AdminApiClient _adminClient;
    private readonly AuthApiClient _authClient;
    private readonly RegisterApiClient _registerApiClient;
    public ApiHelper()
    {
        _client = new ApiClient();
        _adminClient = new AdminApiClient(_client);
        _authClient = new AuthApiClient(_client);
        _registerApiClient = new RegisterApiClient(_client);
    }
    public async Task<RegisterAdminResponse> RegisterAdminFromFormAsync(RegisterAdminForm form)
    {
        var loginResponse = await _authClient.LoginOwnerAsync();
        var token = loginResponse.Token;
        var request = new RegisterAdminRequest(form.Name, 
                                            form.Email, 
                                            token, 
                                            "admin", 
                                            form.Password);
        return await _registerApiClient.RegisterAdminAsync(request);
    }

    public async Task<RegisterResponse> RegisterUserFromFormAsync(RegisterForm form)
    {
        var request= new RegisterCustomerRequest(form.FirstName+" "+form.LastName,
                                                form.Email,form.Password,
                                                form.FullAddress,
                                                form.PhoneNumber,
                                                form.PostalCode);
        return await _registerApiClient.RegisterCustomerAsync(request);
    }


    // }
    public async Task DeleteCustomerAsync(string username, string password)
    {
        var loginResponse = await _authClient.LoginOwnerAsync();
        var token = loginResponse.Token;
        var response = await _authClient.LoginCustomerAsync(username, password);
        await _adminClient.DeleteCustomerAsync(response.Id, token);
    }

    public async Task<string> DeleteAdminAsync(string username, string password)
    {
        var loginResponse = await _authClient.LoginOwnerAsync();
        var token = loginResponse.Token;
        var response = await _authClient.LoginAdminAsync(username, password);
        var deleteResponse = await _adminClient.DeleteAdminAsync(response.Id, token);
        return deleteResponse.Id ?? string.Empty;
    }

    public void Dispose()
    {
        if (_client is IDisposable disposableClient)
        {
            disposableClient.Dispose();
        }
        GC.SuppressFinalize(this);
    }
}
