using Team2QA.Automation.Tests.Models.ApiContracts;

namespace Team2QA.Automation.Tests.Models.Scenarios;

public sealed class TransferScenario
{
    public TestUser Sender { get; set; } = new TestUser();
    public TestUser Receiver { get; set; } = new TestUser();
    public RegisterResponse SenderResponse { get; set; } = new RegisterResponse();
    public RegisterResponse ReceiverResponse { get; set; } = new RegisterResponse();

}
