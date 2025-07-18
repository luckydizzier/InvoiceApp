using System.Collections.ObjectModel;
using InvoiceApp.ViewModels;
using InvoiceApp.Models;
using InvoiceApp.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Linq;

namespace InvoiceApp.Tests
{
    [TestClass]
    public class InvoiceViewModelTests
    {
        private class StubService<T> : IInvoiceService, IInvoiceItemService, IProductService,
            ITaxRateService, ISupplierService, IPaymentMethodService, IChangeLogService, INavigationService
            where T : class, new()
        {
            // Implement all interfaces with no-op or default results
            AppState INavigationService.CurrentState => AppState.Dashboard;
            public event System.EventHandler<AppState>? StateChanged;
            public void ClearSubstates() { }
            public Task DeleteAsync(int id) => Task.CompletedTask;
            public IEnumerable<AppState> GetStatePath() => Enumerable.Empty<AppState>();
            public Task<IEnumerable<Invoice>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Invoice>());
            public Task<IEnumerable<InvoiceItem>> GetAllAsync() => Task.FromResult(Enumerable.Empty<InvoiceItem>());
            public Task<IEnumerable<Product>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Product>());
            public Task<IEnumerable<TaxRate>> GetAllAsync() => Task.FromResult(Enumerable.Empty<TaxRate>());
            public Task<IEnumerable<Supplier>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Supplier>());
            public Task<IEnumerable<PaymentMethod>> GetAllAsync() => Task.FromResult(Enumerable.Empty<PaymentMethod>());
            public Task<Invoice?> GetByIdAsync(int id) => Task.FromResult<Invoice?>(null);
            public Task<InvoiceItem?> GetByIdAsync(int id) => Task.FromResult<InvoiceItem?>(null);
            public Task<Product?> GetByIdAsync(int id) => Task.FromResult<Product?>(null);
            public Task<TaxRate?> GetByIdAsync(int id) => Task.FromResult<TaxRate?>(null);
            public Task<Supplier?> GetByIdAsync(int id) => Task.FromResult<Supplier?>(null);
            public Task<PaymentMethod?> GetByIdAsync(int id) => Task.FromResult<PaymentMethod?>(null);
            public Task<Invoice?> GetLatestForSupplierAsync(int supplierId) => Task.FromResult<Invoice?>(null);
            public Task<Invoice?> GetLatestAsync() => Task.FromResult<Invoice?>(null);
            public Task SaveAsync(Invoice invoice) => Task.CompletedTask;
            public Task SaveAsync(InvoiceItem item) => Task.CompletedTask;
            public Task SaveAsync(Product product) => Task.CompletedTask;
            public Task SaveAsync(TaxRate rate) => Task.CompletedTask;
            public Task SaveAsync(Supplier supplier) => Task.CompletedTask;
            public Task SaveAsync(PaymentMethod method) => Task.CompletedTask;
            public void AddAsync(ChangeLog log) { }
            Task IChangeLogService.AddAsync(ChangeLog log) { AddAsync(log); return Task.CompletedTask; }
            Task<ChangeLog?> IChangeLogService.GetLatestAsync() => Task.FromResult<ChangeLog?>(null);
            public void Pop() { }
            public void PopSubstate() { }
            public void Push(AppState state) { }
            public void PushSubstate(AppState state) { }
            public void SwitchRoot(AppState state) { }
        }

        private static InvoiceViewModel CreateViewModel()
        {
            var stub = new StubService<object>();
            return new InvoiceViewModel(stub, stub, stub, stub, stub, stub, stub,
                new SupplierViewModel(stub), stub);
        }

        [TestMethod]
        public void CalculatesTotals_ForNetMode()
        {
            var vm = CreateViewModel();
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
            var vm = CreateViewModel();
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
