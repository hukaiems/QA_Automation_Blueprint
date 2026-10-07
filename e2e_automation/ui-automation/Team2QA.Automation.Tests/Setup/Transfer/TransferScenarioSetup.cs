using Team2QA.Automation.Tests.Api;
using Team2QA.Automation.Tests.Models.ApiContracts;
using Team2QA.Automation.Tests.Models.Scenarios;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Setup.Transfer;

public class TransferScenarioSetup
{
    private readonly AdminApiClient _adminApi;
    private readonly RegisterApiClient _registerApi;
    private readonly AccountApiClient _accountApi;

    public TransferScenarioSetup(
        AdminApiClient adminApi,
        RegisterApiClient registerApi,
        AccountApiClient accountApi)
    {
        _adminApi = adminApi;
        _registerApi = registerApi;
        _accountApi = accountApi;
    }

    public async Task<TransferScenario> CreateUsersAsync(Dictionary<string, string> data, string adminToken)
    {
        var scenario = new TransferScenario();
        string password = data["Password"];
        int balance = int.Parse(data["Balance"]);

        // ==========================================
        // 1. Setup Sender
        // ==========================================
        scenario.Sender.Email = DataUtils.GenerateRandomEmail(data["Email"]);

        var registeredSender = await _registerApi.RegisterCustomerAsync(
            new RegisterCustomerRequest(
                Name: data["name"],
                Email: scenario.Sender.Email,
                Password: password,
                Address: data["FullAddress"],
                Phone: data["PhoneNumber"],
                Postal: data["PostalCode"]
            )
        );

        scenario.Sender.Id = registeredSender.Id;
        scenario.Sender.Token = registeredSender.Token;

        var reqIdSender = await _accountApi.RequestAccountAsync(
            new RequestAccountRequest(
                Balance: balance,
                UserId: scenario.Sender.Id,
                AccountPassword: password,
                CustomerToken: scenario.Sender.Token
            ),
            scenario.Sender.Token);

        // Approve the Sender's request
        var approveSenderResponse = await _adminApi.ApproveAccountAsync(
            new ApproveAccountRequest(
                Balance: balance,
                UserId: scenario.Sender.Id,
                RequestId: long.Parse(reqIdSender),
                AdminToken: adminToken
            ), adminToken
        );

        if (approveSenderResponse.Id != reqIdSender)
        {
            throw new InvalidOperationException(
                $"Unexpected approval response ID for Sender. " +
                $"Expected: {approveSenderResponse.Id}; actual: {reqIdSender}."
            );
        }

        // because the api will give the different transfering account id back so we need to request 1 more to get it
        var reqIdSenderReal = await _accountApi.RequestAccountAsync(
            new RequestAccountRequest(
                Balance: balance,
                UserId: scenario.Sender.Id,
                AccountPassword: password,
                CustomerToken: scenario.Sender.Token
            ),
            scenario.Sender.Token, "approved");

        // append the sender correct id
        scenario.Sender.AccountRequestId = reqIdSenderReal;

        // ==========================================
        // 2. Setup Receiver
        // ==========================================
        scenario.Receiver.Email = DataUtils.GenerateRandomEmail(data["Email"]);

        var registeredReceiver = await _registerApi.RegisterCustomerAsync(
            new RegisterCustomerRequest(
                Name: data["name"],
                Email: scenario.Receiver.Email,
                Password: password,
                Address: data["FullAddress"],
                Phone: data["PhoneNumber"],
                Postal: data["PostalCode"]
            )
        );

        scenario.Receiver.Id = registeredReceiver.Id;
        scenario.Receiver.Token = registeredReceiver.Token;

        var reqIdReceiver = await _accountApi.RequestAccountAsync(
            new RequestAccountRequest(
                Balance: balance,
                UserId: scenario.Receiver.Id,
                AccountPassword: password,
                CustomerToken: scenario.Receiver.Token
            ),
            scenario.Receiver.Token);

        // Approve the Receiver's request
        var approveReceiverResponse = await _adminApi.ApproveAccountAsync(
            new ApproveAccountRequest(
                Balance: balance,
                UserId: scenario.Receiver.Id,
                RequestId: long.Parse(reqIdReceiver),
                AdminToken: adminToken
            ), adminToken
        );

        if (approveReceiverResponse.Id != reqIdReceiver)
        {
            throw new InvalidOperationException(
                $"Unexpected approval response ID for Receiver. " +
                $"Expected: {approveReceiverResponse.Id}; actual: {reqIdReceiver}."
            );
        }

        var reqIdReceiverTrue = await _accountApi.RequestAccountAsync(
            new RequestAccountRequest(
                Balance: balance,
                UserId: scenario.Receiver.Id,
                AccountPassword: password,
                CustomerToken: scenario.Receiver.Token
            ),
            scenario.Receiver.Token, "approved");

        // append request Id of Receiver into the Transfer scenario
        scenario.Receiver.AccountRequestId = reqIdReceiverTrue;

        // 3. Return the fully scenario
        return scenario;
    }

    public async Task CleanupAsync(string senderId, string receiverId, string adminToken)
    {
        if (!string.IsNullOrEmpty(senderId))
        {
            await _adminApi.DeleteCustomerAsync(senderId, adminToken);
        }

        if (!string.IsNullOrEmpty(receiverId))
        {
            await _adminApi.DeleteCustomerAsync(receiverId, adminToken);
        }
    }
}


