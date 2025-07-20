using OpenQA.Selenium.Appium;

namespace AppiumTests
{
    internal static class AppiumOptionsExtensions
    {
        public static void AddAdditionalCapability(this AppiumOptions options, string name, object value)
        {
            options.AddAdditionalAppiumOption(name, value);
        }
    }
}
