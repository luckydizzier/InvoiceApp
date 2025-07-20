using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;
using FlaUI.UIA3;
using System;
using System.IO;

#nullable enable

namespace FlaUITests
{
    [TestClass]
    public class UITests
    {
        private const string HarnessExe = "UITestHarness.exe";
        private UIA3Automation? _automation;

        [TestInitialize]
        public void Setup()
        {
            _automation = new UIA3Automation();
        }

        private Application LaunchHarness(string arguments = "")
        {
            var exePath = Path.Combine(TestContext!.DeploymentDirectory, HarnessExe);
            return string.IsNullOrEmpty(arguments)
                ? Application.Launch(exePath)
                : Application.Launch(exePath, arguments);
        }

        [TestMethod]
        public void SampleDataDialogHasOkButton()
        {
            using var app = LaunchHarness("dialog");
            var window = app.GetMainWindow(_automation!);
            var ok = window.FindFirstDescendant(cf => cf.ByAutomationId("OkButton"));
            Assert.IsNotNull(ok);
        }

        [TestMethod]
        public void SampleDataDialogHasCancelButton()
        {
            using var app = LaunchHarness("dialog");
            var window = app.GetMainWindow(_automation!);
            var cancel = window.FindFirstDescendant(cf => cf.ByAutomationId("CancelButton"));
            Assert.IsNotNull(cancel);
        }

        [TestMethod]
        public void MainWindowHasNewInvoiceButton()
        {
            using var app = LaunchHarness();
            var window = app.GetMainWindow(_automation!);
            var newButton = window.FindFirstDescendant(cf => cf.ByAutomationId("NewInvoiceButton"));
            Assert.IsNotNull(newButton);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _automation?.Dispose();
        }

        public TestContext TestContext { get; set; } = null!;
    }
}
