using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
namespace Team2QA.Automation.Tests.Utils;

public class ExtentManager
{
    private static readonly Lazy<ExtentReports> _lazy = new Lazy<ExtentReports>(() => new ExtentReports());
    public static ExtentReports Instance { get { return _lazy.Value; } }
    public static string ReportDirectory { get; private set; } = "";

    static ExtentManager()
    {
        string workingDirectory = Environment.CurrentDirectory;
        string projectDirectory = Directory.GetParent(workingDirectory)?.Parent?.Parent?.FullName ?? workingDirectory;
        ReportDirectory = Path.Combine(projectDirectory, "TestReports");
        Directory.CreateDirectory(ReportDirectory);
        var reportPath = Path.Combine(ReportDirectory, "team2_ui_report.html");
        var htmlReporter = new ExtentSparkReporter(reportPath)
        {
            Config =
            {
                DocumentTitle="Test Report"
            }
        };
        Instance.AttachReporter(htmlReporter);
    }

    private ExtentManager()
    {
    }
}
