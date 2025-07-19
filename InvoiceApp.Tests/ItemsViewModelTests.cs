using System.Collections.ObjectModel;
using InvoiceApp.Models;
using InvoiceApp.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class ItemsViewModelTests
    {
        private ItemsViewModel CreateVm(Invoice invoice)
        {
            return TestHelpers.CreateItemsViewModel(invoice);
        }

        [TestMethod]
        public void TotalsUpdate_WhenAddingAndRemovingItems()
        {
            var invoice = new Invoice { IsGross = false };
            var vm = CreateVm(invoice);

            var item1 = new InvoiceItemViewModel(new InvoiceItem
            {
                Quantity = 2,
                UnitPrice = 100m,
                TaxRate = new TaxRate { Percentage = 27m }
            }) { TaxRatePercentage = 27m };

            var item2 = new InvoiceItemViewModel(new InvoiceItem
            {
                Quantity = 1,
                UnitPrice = 50m,
                TaxRate = new TaxRate { Percentage = 5m }
            }) { TaxRatePercentage = 5m };

            vm.Items = new ObservableCollection<InvoiceItemViewModel> { item1 };

            Assert.AreEqual(200m, vm.TotalNet);
            Assert.AreEqual(54m, vm.TotalVat);
            Assert.AreEqual(254m, vm.TotalGross);

            vm.Items.Add(item2);

            Assert.AreEqual(250m, vm.TotalNet);
            Assert.AreEqual(56.5m, vm.TotalVat);
            Assert.AreEqual(306.5m, vm.TotalGross);

            vm.Items.Remove(item1);

            Assert.AreEqual(50m, vm.TotalNet);
            Assert.AreEqual(2.5m, vm.TotalVat);
            Assert.AreEqual(52.5m, vm.TotalGross);
        }
    }
}
