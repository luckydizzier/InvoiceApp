using System.Collections.ObjectModel;
using InvoiceApp.ViewModels;
using InvoiceApp.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class InvoiceViewModelTests
    {

        [TestMethod]
        public void CalculatesTotals_ForNetMode()
        {
            var vm = TestHelpers.CreateInvoiceViewModel();
            vm.SelectedInvoice = new Invoice { IsGross = false };
            var items = new ObservableCollection<InvoiceItemViewModel>
            {
                new InvoiceItemViewModel(new InvoiceItem
                {
                    Quantity = 2,
                    UnitPrice = 100,
                    TaxRate = new TaxRate { Percentage = 27m }
                }) { TaxRatePercentage = 27m },
                new InvoiceItemViewModel(new InvoiceItem
                {
                    Quantity = 1,
                    UnitPrice = 50,
                    TaxRate = new TaxRate { Percentage = 5m }
                }) { TaxRatePercentage = 5m }
            };
            vm.Items = items;
            vm.IsGrossCalculation = false;

            Assert.AreEqual(250m, vm.TotalNet);
            Assert.AreEqual(56.5m, vm.TotalVat);
            Assert.AreEqual(306.5m, vm.TotalGross);
        }

        [TestMethod]
        public void CalculatesTotals_ForGrossMode()
        {
            var vm = TestHelpers.CreateInvoiceViewModel();
            vm.SelectedInvoice = new Invoice { IsGross = true };
            var items = new ObservableCollection<InvoiceItemViewModel>
            {
                new InvoiceItemViewModel(new InvoiceItem
                {
                    Quantity = 2,
                    UnitPrice = 127,
                    TaxRate = new TaxRate { Percentage = 27m }
                }) { TaxRatePercentage = 27m },
                new InvoiceItemViewModel(new InvoiceItem
                {
                    Quantity = 1,
                    UnitPrice = 52.5m,
                    TaxRate = new TaxRate { Percentage = 5m }
                }) { TaxRatePercentage = 5m }
            };
            vm.Items = items;
            vm.IsGrossCalculation = true;

            Assert.AreEqual(250m, vm.TotalNet);
            Assert.AreEqual(56.5m, vm.TotalVat);
            Assert.AreEqual(306.5m, vm.TotalGross);
        }
    }
}
