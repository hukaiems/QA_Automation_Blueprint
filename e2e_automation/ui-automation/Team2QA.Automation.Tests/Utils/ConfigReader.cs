using Microsoft.Extensions.Configuration;
namespace Team2QA.Automation.Tests.Utils;

public static class ConfigReader
{
    // cached data into mem 
    private static readonly Lazy<RootSettings> _instance = new Lazy<RootSettings>(BuildConfiguration);
    // RootSettings to store all data in appsettings
    public static RootSettings Instance => _instance.Value;
    public static AppSettings App => Instance.AppSettings;
    public static BrowserSettings Browser => Instance.Browser;
    public static AdminSettings Admin => Instance.Admin;
    private static RootSettings BuildConfiguration()
    {
        string projectDir = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "../../../"));
        IConfigurationRoot config = new ConfigurationBuilder()
            .SetBasePath(projectDir)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddUserSecrets<RootSettings>()
            .AddEnvironmentVariables()
            .Build();

        var settings = new RootSettings();
        config.Bind(settings);
        return settings;
    }
}

public class RootSettings
{
    public AppSettings AppSettings { get; set; } = new AppSettings();
    public BrowserSettings Browser { get; set; } = new BrowserSettings();
    public AdminSettings Admin { get; set; } = new AdminSettings();
}

public class AdminSettings
{
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}


public class AppSettings
{
    public string BaseUrl { get; set; } = string.Empty;
    public double ImplicitWait { get; set; }
    public double ExplicitWait { get; set; }
    public int PollingInterval {get; set;}
}

public class BrowserSettings
{
    public string Type { get; set; } = string.Empty;
    public bool Headless { get; set; }
    public bool RunRemote { get; set; }
    public string RemoteUrl { get; set; } = "http://localhost:4444/wd/hub";
}
