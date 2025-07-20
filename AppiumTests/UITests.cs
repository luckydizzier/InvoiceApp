using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Windows;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace AppiumTests
{
    [TestClass]
    public class UITests
    {
        private const string WinAppDriverUrl = "http://127.0.0.1:4723";
        private const string HarnessExe = "UITestHarness.exe";
        private WindowsDriver<WindowsElement>? _session;
        private Process? _process;

        [TestInitialize]
        public void Setup()
        {
            var exePath = Path.Combine(TestContext.DeploymentDirectory, HarnessExe);
            _process = Process.Start(exePath, "dialog");
            var options = new AppiumOptions();
            options.AddAdditionalCapability("app", exePath);
            options.AddAdditionalCapability("deviceName", "WindowsPC");
            _session = new WindowsDriver<WindowsElement>(new Uri(WinAppDriverUrl), options);
        }

        [TestMethod]
        public void SampleDataDialogHasOkButton()
        {
            var ok = _session!.FindElementByAccessibilityId("OkButton");
            Assert.IsNotNull(ok);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _session?.Quit();
            if (_process != null && !_process.HasExited)
            {
                _process.Kill();
                _process.WaitForExit();
            }
        }

        public TestContext TestContext { get; set; } = null!;
    }
}
