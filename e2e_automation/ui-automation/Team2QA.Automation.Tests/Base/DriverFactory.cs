using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Remote;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Base;

public enum BrowserType
{
    Chrome,
    Firefox,
    Edge
}

public class DriverFactory
{
    private readonly RootSettings _settings;

    public DriverFactory(RootSettings settings)
    {
        _settings = settings;
    }

    public async Task<IWebDriver> CreateDriverAsync(BrowserType browserType, bool headless)
    {
        bool isRemote = _settings.Browser.RunRemote;

        if (isRemote)
        {
            string remoteUrl = _settings.Browser.RemoteUrl;
            if (string.IsNullOrWhiteSpace(remoteUrl))
            {
                throw new ArgumentException("RemoteUrl is required when RunRemote is true.");
            }

            return await CreateRemoteDriverAsync(browserType, headless, remoteUrl);
        }

        return browserType switch
        {
            BrowserType.Chrome => new ChromeDriver(BuildChromeOptions(headless)),
            BrowserType.Firefox => new FirefoxDriver(BuildFirefoxOptions(headless)),
            BrowserType.Edge => new EdgeDriver(BuildEdgeOptions(headless)),
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), "Unsupported browser type"),
        };
    }

    private static async Task WaitForRemoteServerAsync(string remoteUrl)
    {
        var uri = new Uri(remoteUrl);
        var statusUrl = $"{uri.Scheme}://{uri.Host}:{uri.Port}/status";

        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);

        for (int i = 0; i < 30; i++)
        {
            try
            {
                var response = await client.GetAsync(statusUrl);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"[INFO] Remote WebDriver server at {statusUrl} is ready.");
                    return;
                }
            }
            catch
            {
                // Bỏ qua lỗi kết nối trong lúc poll
            }
            Console.WriteLine($"[INFO] Waiting for Remote WebDriver server... ({i + 1}/30)");
            await Task.Delay(2000);
        }
        Console.WriteLine("[WARN] Remote WebDriver server polling timed out. Attempting to connect anyway...");
    }

    private static async Task<IWebDriver> CreateRemoteDriverAsync(BrowserType browserType, bool headless, string remoteUrl)
    {
        DriverOptions options = browserType switch
        {
            BrowserType.Chrome => BuildChromeOptions(headless),
            BrowserType.Firefox => BuildFirefoxOptions(headless),
            BrowserType.Edge => BuildEdgeOptions(headless),
            _ => throw new ArgumentOutOfRangeException(nameof(browserType), "Unsupported browser type"),
        };

        // Chờ cho đến khi Selenium Server khởi động xong
        await WaitForRemoteServerAsync(remoteUrl);

        return new RemoteWebDriver(new Uri(remoteUrl), options.ToCapabilities(), TimeSpan.FromMinutes(3));
    }

    private static ChromeOptions BuildChromeOptions(bool headless)
    {
        var options = new ChromeOptions();
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--disable-gpu");
        options.AddArgument("--start-maximized");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-infobars");
        if (headless)
        {
            options.AddArgument("--headless=new");
        }

        return options;
    }

    private static FirefoxOptions BuildFirefoxOptions(bool headless)
    {
        var options = new FirefoxOptions();
        options.AddArgument("--width=1920");
        options.AddArgument("--height=1080");
        if (headless)
        {
            options.AddArgument("--headless");
        }

        return options;
    }

    private static EdgeOptions BuildEdgeOptions(bool headless)
    {
        var options = new EdgeOptions();
        options.AddArgument("--start-maximized");
        options.AddArgument("--window-size=1920,1080");
        options.AddArgument("--disable-gpu");
        if (headless)
        {
            options.AddArgument("--headless=new");
        }

        return options;
    }
}
