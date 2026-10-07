using AventStack.ExtentReports;
using NUnit.Framework.Interfaces;
using OpenQA.Selenium;
using Team2QA.Automation.Tests.Base;
using Team2QA.Automation.Tests.Utils;
using System.Collections.Concurrent;
namespace Team2QA.Automation.Tests.Tests;

public abstract class BaseTest
{
    protected ExtentReports Extent = null!;
    protected RootSettings Settings = null!;
    protected static DriverFactory driverFactory;
    protected static ConcurrentDictionary<string, IWebDriver> DriversLocal = new();
    protected static IWebDriver Driver => DriversLocal[TestContext.CurrentContext.Test.ID];
    protected ExtentTest TestNode = null!;

    [OneTimeSetUp]
    public void OneTimeSetUp()
    {
        Extent = ExtentManager.Instance;
        Settings = ConfigReader.Instance;
        driverFactory = new DriverFactory(Settings);
    }

    [OneTimeTearDown]
    public void OneTimeTearDown() => Extent.Flush();


    [SetUp]
    public async Task Setup()
    {
        var browserStr = TestContext.Parameters.Get("Browser", Settings.Browser.Type);
        BrowserType browserType = Enum.Parse<BrowserType>(browserStr, true);
        bool headless = Settings.Browser.Headless;
        var driver = await driverFactory.CreateDriverAsync(browserType, headless);
        DriversLocal[TestContext.CurrentContext.Test.ID] = driver; // assign ID of each driver for each Thread
        TestNode = ExtentTestManager.CreateTest(BuildExtentTestName());
        if (!headless)
        {
            Driver.Manage().Window.Maximize();
        }
    }

    private static string BuildExtentTestName()
    {
        var name = TestContext.CurrentContext.Test.Name;
        var tcId = TestContext.CurrentContext.Test.Properties.Get("TC_ID") as string;
        return string.IsNullOrWhiteSpace(tcId) ? name : $"[{tcId}] {name}";
    }
    [TearDown]
    public void TearDown()
    {
        var status = TestContext.CurrentContext.Result.Outcome.Status;
        var stacktrace = string.IsNullOrEmpty(TestContext.CurrentContext.Result.StackTrace)
                ? ""
                : string.Format("<pre>{0}</pre>", TestContext.CurrentContext.Result.StackTrace);
        var logstatus = status switch
        {
            TestStatus.Failed => Status.Fail,
            TestStatus.Inconclusive => Status.Warning,
            TestStatus.Skipped => Status.Skip,
            _ => Status.Pass,
        };

        var message = "Test ended with " + logstatus + stacktrace;

        if (logstatus == Status.Fail)
        {
            var screenshotPath = CaptureScreenshot(TestContext.CurrentContext.Test.Name);
            if (screenshotPath != null)
            {
                ExtentTestManager.GetTest()?.Log(logstatus, message,
                    MediaEntityBuilder.CreateScreenCaptureFromPath(screenshotPath)
                                      .Build());
            }
            else
            {
                ExtentTestManager.GetTest()?.Log(logstatus, message);
            }
        }
        else
        {
            ExtentTestManager.GetTest()?.Log(logstatus, message);
        }

        if (DriversLocal.TryRemove(TestContext.CurrentContext.Test.ID, out var driver))
        {
            driver.Quit();
            driver.Dispose();
        }
    }

    private static string? CaptureScreenshot(string testName)
    {
        try
        {
            var testDir = ExtentManager.ReportDirectory;
            var screenshotsDir = Path.Combine(testDir, "Screenshots");
            Directory.CreateDirectory(screenshotsDir);
            var filePath = Path.Combine(screenshotsDir, $"{testName}_{DateTime.Now:yyyyMMdd_HHmmss}.png");
            ((ITakesScreenshot)Driver).GetScreenshot().SaveAsFile(filePath);
            return Path.GetRelativePath(testDir, filePath);
        }
        catch (Exception ex)
        {
            ExtentTestManager.GetTest()?
                             .Log(Status.Warning, $"Could not capture screenshot: {ex.Message}");
            return null;
        }
    }
}
