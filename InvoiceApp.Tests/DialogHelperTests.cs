using InvoiceApp;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class DialogHelperTests
    {
        [TestMethod]
        public void ShowConfirmation_UsesHandlerWhenProvided()
        {
            bool called = false;
            DialogHelper.ConfirmationHandler = (msg, title) => { called = true; return true; };

            var result = DialogHelper.ShowConfirmation("M", "T");

            DialogHelper.ConfirmationHandler = null;

            Assert.IsTrue(called);
            Assert.IsTrue(result);
        }
    }
}
