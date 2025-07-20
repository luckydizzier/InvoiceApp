using InvoiceApp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class AmountCalculatorTests
    {
        [TestMethod]
        public void Calculate_GrossInvoice_ReturnsExpectedValues()
        {
            var result = AmountCalculator.Calculate(2m, 127m, 27m, true);

            Assert.AreEqual(200m, result.Net);
            Assert.AreEqual(54m, result.Vat);
            Assert.AreEqual(254m, result.Gross);
        }

        [TestMethod]
        public void Calculate_NetInvoice_ReturnsExpectedValues()
        {
            var result = AmountCalculator.Calculate(2m, 100m, 27m, false);

            Assert.AreEqual(200m, result.Net);
            Assert.AreEqual(54m, result.Vat);
            Assert.AreEqual(254m, result.Gross);
        }
    }
}
