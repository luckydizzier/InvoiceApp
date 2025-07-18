using System.Collections.ObjectModel;
using InvoiceApp.ViewModels;
using InvoiceApp.Models;
using InvoiceApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class ProductViewModelTests
    {
        private static ProductViewModel CreateVm()
        {
            var stub = new StubService<object>();
            return new ProductViewModel(stub, stub, stub, stub, new StatusService());
        }

        [TestMethod]
        public void NetChange_UpdatesGross_WhenNetMode()
        {
            var vm = CreateVm();
            var rate = new TaxRate { Id = 1, Percentage = 27m };
            vm.TaxRates = new ObservableCollection<TaxRate> { rate };
            var product = new Product { TaxRate = rate, TaxRateId = rate.Id };
            vm.Products = new ObservableCollection<Product> { product };
            vm.SelectedProduct = product;
            vm.IsGrossInput = false;

            product.Net = 100m;

            Assert.AreEqual(127m, product.Gross);
        }

        [TestMethod]
        public void GrossChange_UpdatesNet_WhenGrossMode()
        {
            var vm = CreateVm();
            var rate = new TaxRate { Id = 1, Percentage = 27m };
            vm.TaxRates = new ObservableCollection<TaxRate> { rate };
            var product = new Product { TaxRate = rate, TaxRateId = rate.Id };
            vm.Products = new ObservableCollection<Product> { product };
            vm.SelectedProduct = product;
            vm.IsGrossInput = true;

            product.Gross = 127m;

            Assert.AreEqual(100m, product.Net);
        }
    }
}
