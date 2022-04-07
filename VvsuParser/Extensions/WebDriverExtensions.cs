using System;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;

namespace VvsuParser.Extensions;

public static class WebDriverExtensions
{
    public static Task<IWebElement> WaitToFindElement(this IWebDriver driver, By by, int timeoutInSeconds)
    {
        return Task.Run(async () =>
        {
            if (timeoutInSeconds > 0)
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(timeoutInSeconds));
                wait.IgnoreExceptionTypes(typeof(NoSuchElementException));
                await Task.Delay(TimeSpan.FromSeconds(timeoutInSeconds));
                return wait.Until(drv => drv.FindElement(by));
            }

            return driver.FindElement(by);
        });
    }
}