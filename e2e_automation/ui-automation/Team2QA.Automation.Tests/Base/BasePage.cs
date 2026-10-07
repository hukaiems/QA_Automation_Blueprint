using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Team2QA.Automation.Tests.Utils;

namespace Team2QA.Automation.Tests.Base;

public abstract class BasePage
{
    protected readonly IWebDriver Driver;
    protected readonly WebDriverWait Wait;
    protected readonly RootSettings rootSettings;
    protected string? PageUrl;
    protected BasePage(IWebDriver driver, RootSettings settings)
    {
        Driver = driver;
        rootSettings = settings;
        Wait = new WebDriverWait(driver,
                    TimeSpan.FromSeconds(rootSettings.AppSettings.ExplicitWait));
        if (rootSettings.AppSettings.ImplicitWait > 0)
        {
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(
                            rootSettings.AppSettings.ImplicitWait);
        }
    }

    /// <summary>
    /// Builds an absolute URL by appending <paramref name="path"/> to the configured BaseUrl.
    /// </summary>
    /// <param name="path">A route path constant from <see cref="Routes"/> (e.g. Routes.Login).</param>
    protected string BuildUrl(string path)
        => $"{rootSettings.AppSettings.BaseUrl.TrimEnd('/')}{path}";

    /// <summary>Navigates the browser to this page's <see cref="PageUrl"/>.</summary>
    public void NavigateTo() => Driver.Navigate().GoToUrl(PageUrl ?? throw new InvalidOperationException("PageUrl is not set."));

    /// <summary>Navigates the browser to an arbitrary absolute URL.</summary>
    public void NavigateToUrl(string url) => Driver.Navigate().GoToUrl(url);
    protected void Refresh() => Driver.Navigate().Refresh();
    private WebDriverWait GetWebDriverWait(TimeSpan? timeout) => new WebDriverWait(Driver, timeout ??
                                            TimeSpan.FromSeconds(rootSettings.AppSettings.ExplicitWait));

    protected IWebElement? Click(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        ScrollIntoViewJS(element);
        element.Click();
        return element;
    }
    protected void TypeTextSafe(IWebElement? element, string text)
    {
        if (element is null)
        {
            return;
        }

        ScrollToCenter(element);
        element.Clear();
        element.SendKeys(text);
    }


    protected IWebElement? TypeText(IWebElement? element, string text)
    {
        if (element == null)
        {
            return null;
        }

        Click(element);
        element.Clear();
        element.SendKeys(text);
        return element;
    }
    protected IWebElement? Hover(IWebElement element)
    {
        if (element is not null)
        {
            new Actions(Driver).MoveToElement(element).Perform();
        }

        return element;
    }


    // Fluent Wait
    /* 
    Fluent Wait Components
    1. Inside wait (timer) need the timeout (explicit time) and polling interval, the retry
    2. Ignore which exceptions when polling
    3. Wait until to return element
    */
    
    protected IWebElement? WaitForElementExist(By locator, TimeSpan? timeout = null)
    {
        // apply fluent wait
        var wait = new DefaultWait<IWebDriver>(Driver)
        {
            Timeout = timeout ?? TimeSpan.FromSeconds(rootSettings.AppSettings.ExplicitWait),
            PollingInterval = TimeSpan.FromMilliseconds(rootSettings.AppSettings.PollingInterval)
        };

        // Ignore exceptions
        wait.IgnoreExceptionTypes(
            typeof(NoSuchElementException),
            typeof(StaleElementReferenceException),
            typeof(ElementNotInteractableException)
        );

        return wait.Until(d =>
        {
            var element = d.FindElement(locator);
            return (element.Displayed && element.Enabled) ? element : null;
        });
    }
    protected IWebElement WaitForClickable(By locator, TimeSpan? timeout = null)
    {
        try
        {
            var element = GetWebDriverWait(timeout).Until(d =>
            {
                try
                {
                    var ele = d.FindElement(locator);
                    if (ele.Displayed && ele.Enabled)
                    {
                        return ele;
                    }
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            return element;
        }
        catch (WebDriverTimeoutException)
        {
            throw;
        }
    }

    protected IWebElement WaitForVisible(By locator, TimeSpan? timeout = null)
    {
        try
        {
            var element = GetWebDriverWait(timeout).Until(d =>
            {
                try
                {
                    var ele = d.FindElement(locator);
                    if (ele.Displayed)
                    {
                        return ele;
                    }
                    return null;
                }
                catch (StaleElementReferenceException)
                {
                    return null;
                }
                catch (NoSuchElementException)
                {
                    return null;
                }
            });
            return element;
        }
        catch (WebDriverTimeoutException)
        {
            throw;
        }
    }

    protected string? GetHtml5ValidationMessage(IWebElement element)
    {
        return element.GetAttribute("validationMessage");
    }


    protected IWebElement? DoubleClick(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        new Actions(Driver).ScrollToElement(element).DoubleClick().Perform();
        return element;
    }

    protected IWebElement? RightClick(IWebElement? element)
    {
        if (element is null)
        {
            return null;
        }

        new Actions(Driver).ScrollToElement(element).ContextClick().Perform();
        return element;
    }

    protected void DragAndDrop(IWebElement? source, IWebElement? target)
    {
        if (source is null || target is null)
        {
            return;
        }

        new Actions(Driver).ScrollToElement(source).DragAndDrop(source, target).Perform();
    }


    // // ---- JavaScript helpers ----

    protected void JavaScriptClick(IWebElement? element)
    {
        if (element is null)
        {
            return;
        }

        ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].click();", element);
    }
    protected void ScrollToBottom()
        => ((IJavaScriptExecutor)Driver).ExecuteScript("window.scrollTo(0, document.body.scrollHeight);");
    protected void ScrollToTop()
        => ((IJavaScriptExecutor)Driver).ExecuteScript("window.scrollTo(0, 0);");

    protected void SwitchToTab(int index)
    {
        var tabs = Driver.WindowHandles;
        if (index < 0 || index >= tabs.Count)
        {
            return;
        }

        Driver.SwitchTo().Window(tabs[index]);
    }
    protected IWebElement ScrollIntoViewJS(IWebElement element)
    {
        try
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript(
                    "arguments[0].scrollIntoView(true);", element);
            return element;
        }
        catch (WebDriverException)
        {
            return element;
        }
    }
    protected IWebElement ScrollToCenter(IWebElement element)
    {
        ((IJavaScriptExecutor)Driver).ExecuteScript(
            "arguments[0].scrollIntoView({block:'center'});", element);
        return element;
    }
    protected void AddCookie(string name, string value)
    {
        Driver.Manage().Cookies.AddCookie(new Cookie(name, value));
    }
    public Cookie? GetCookie(string name)
    {
        var cookie = Driver.Manage().Cookies.GetCookieNamed(name);
        return cookie;
    }
    public void DeleteCookie(string name)
    {
        var cookie = GetCookie(name);
        if (cookie != null)
        {
            Driver.Manage().Cookies.DeleteCookie(cookie);
        }
    }
    public void DeleteAllCookies()
    {
        Driver.Manage().Cookies.DeleteAllCookies();
    }
}
