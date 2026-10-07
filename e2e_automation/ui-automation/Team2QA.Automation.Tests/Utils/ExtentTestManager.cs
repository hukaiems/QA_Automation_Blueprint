using AventStack.ExtentReports;
using System.Collections.Concurrent;

namespace Team2QA.Automation.Tests.Utils;

public class ExtentTestManager
{
    private static readonly ConcurrentDictionary<string, ExtentTest> _tests = new();
    private static readonly ExtentReports _extent = ExtentManager.Instance;
    public static ExtentTest? GetTest()
    {
        var id = TestContext.CurrentContext.Test.ID;
        return _tests.TryGetValue(id, out var test) ? test : null;
    }
    public static ExtentTest CreateTest(string name)
    {
        var test = _extent.CreateTest(name);
        var id = TestContext.CurrentContext.Test.ID;
        _tests[id] = test;
        return test;
    }
}
