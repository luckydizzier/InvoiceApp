using InvoiceApp.Helpers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class InvoiceItemCalculationTests
    {
        [TestMethod]
        public void CalculatesAmounts_ForNetMode()
        {
            var amounts = AmountCalculator.Calculate(2m, 100m, 27m, false);
            Assert.AreEqual(200m, amounts.Net);
            Assert.AreEqual(54m, amounts.Vat);
            Assert.AreEqual(254m, amounts.Gross);
        }

        [TestMethod]
        public void CalculatesAmounts_ForGrossMode()
        {
            var amounts = AmountCalculator.Calculate(2m, 127m, 27m, true);
            Assert.AreEqual(200m, amounts.Net);
            Assert.AreEqual(54m, amounts.Vat);
            Assert.AreEqual(254m, amounts.Gross);
        }
    }
}
