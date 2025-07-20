using Microsoft.VisualStudio.TestTools.UnitTesting;
using FlaUI.Core;
using FlaUI.UIA3;
using System;
using System.Diagnostics;
using System.IO;

#nullable enable

namespace FlaUITests
{
    [TestClass]
    public class UITests
    {
        private const string HarnessExe = "UITestHarness.exe";
        private Application? _application;
        private UIA3Automation? _automation;

        [TestInitialize]
        public void Setup()
        {
            var exePath = Path.Combine(TestContext!.DeploymentDirectory, HarnessExe);
            var psi = new ProcessStartInfo(exePath, "dialog");
            _application = Application.Launch(psi);
            _automation = new UIA3Automation();
        }

        [TestMethod]
        public void SampleDataDialogHasOkButton()
        {
            var window = _application!.GetMainWindow(_automation!);
            var ok = window.FindFirstDescendant(cf => cf.ByAutomationId("OkButton"));
            Assert.IsNotNull(ok);
        }

        [TestCleanup]
        public void Cleanup()
        {
            _automation?.Dispose();
            _application?.Close();
        }

        public TestContext TestContext { get; set; } = null!;
    }
}
