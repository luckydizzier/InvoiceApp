using InvoiceApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class StatusServiceTests
    {
        [TestMethod]
        public void Show_SetsMessageAndNotifies()
        {
            var service = new StatusService();
            bool notified = false;
            service.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(StatusService.Message))
                    notified = true;
            };

            service.Show("hello");

            Assert.AreEqual("hello", service.Message);
            Assert.IsTrue(notified);
        }
    }
}
