using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using InvoiceApp.Models;
using InvoiceApp.Services;
using InvoiceApp.ViewModels;
using Moq;
using Xunit;

namespace InvoiceApp.Tests
{
    public class InvoiceViewModelTests
    {
        [Fact]
        public async Task SaveAsync_SavesProductsAndItems()
        {
            var invoiceSvc = new Mock<IInvoiceService>();
            var itemSvc = new Mock<IInvoiceItemService>();
            var productSvc = new Mock<IProductService>();
            var logSvc = new Mock<IChangeLogService>();

            var vm = new InvoiceViewModel(invoiceSvc.Object, itemSvc.Object, productSvc.Object, logSvc.Object);
            var product = new Product { Id = 1, Name = "p" };
            vm.Products = new ObservableCollection<Product> { product };
            var item = new InvoiceItem { Id = 2, Product = product, ProductId = 1 };
            vm.Items = new ObservableCollection<InvoiceItem> { item };
            vm.SelectedInvoice = new Invoice { Id = 3, Items = new List<InvoiceItem>() };

            vm.SaveCommand.Execute(null);
            await Task.Delay(10);

            productSvc.Verify(s => s.SaveAsync(product), Times.Once);
            itemSvc.Verify(s => s.SaveAsync(item), Times.Once);
            invoiceSvc.Verify(s => s.SaveAsync(It.IsAny<Invoice>()), Times.Once);
        }
    }
}
