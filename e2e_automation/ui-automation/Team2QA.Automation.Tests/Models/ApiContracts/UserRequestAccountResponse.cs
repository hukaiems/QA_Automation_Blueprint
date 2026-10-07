namespace Team2QA.Automation.Tests.Models.ApiContracts;

public class UserRequestAccountResponse
{
    public string? name { get; set; }
    public string? email { get; set; }
    public string? address { get; set; }
    public string? id { get; set; }
    public int accountsCount { get; set; }
    public DateTime createdAt { get; set; }
    public int userStatus { get; set; }
    public int postal { get; set; }
    public int phone { get; set; }
    public List<string>? accounts { get; set; }
    public List<Notification>? notifications { get; set; }
}

public class Datum
{
    public string? account_id { get; set; }
}

public class Notification
{
    public string? type { get; set; }
    public string? title { get; set; }
    public string? message { get; set; }
    public bool isSeen { get; set; }
    public List<Datum>? data { get; set; }
    public string? _id { get; set; }
    public DateTime createdAt { get; set; }
    public DateTime updatedAt { get; set; }
}
