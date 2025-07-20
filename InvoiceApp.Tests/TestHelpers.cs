using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InvoiceApp.Domain;
using InvoiceApp.Services;
using InvoiceApp.ViewModels;

namespace InvoiceApp.Tests
{
    internal class StubService<T> : IInvoiceService, IInvoiceItemService, IProductService,
        ITaxRateService, ISupplierService, IPaymentMethodService, IChangeLogService, INavigationService
        where T : class, new()
    {
        AppState INavigationService.CurrentState => AppState.Dashboard;
        public event System.EventHandler<AppState>? StateChanged;
        public void ClearSubstates() { }
        public Task DeleteAsync(int id) => Task.CompletedTask;
        public IEnumerable<AppState> GetStatePath() => Enumerable.Empty<AppState>();
        public Task<IEnumerable<Invoice>> GetAllAsync() => Task.FromResult(Enumerable.Empty<Invoice>());
        public Task<IEnumerable<Invoice>> GetHeadersAsync() => Task.FromResult(Enumerable.Empty<Invoice>());
        Task<IEnumerable<InvoiceItem>> IInvoiceItemService.GetAllAsync() => Task.FromResult(Enumerable.Empty<InvoiceItem>());
        Task<IEnumerable<Product>> IProductService.GetAllAsync() => Task.FromResult(Enumerable.Empty<Product>());
        Task<IEnumerable<TaxRate>> ITaxRateService.GetAllAsync() => Task.FromResult(Enumerable.Empty<TaxRate>());
        Task<IEnumerable<Supplier>> ISupplierService.GetAllAsync() => Task.FromResult(Enumerable.Empty<Supplier>());
        Task<IEnumerable<PaymentMethod>> IPaymentMethodService.GetAllAsync() => Task.FromResult(Enumerable.Empty<PaymentMethod>());
        public Task<Invoice?> GetByIdAsync(int id) => Task.FromResult<Invoice?>(null);
        public Task<Invoice?> GetDetailsAsync(int id) => Task.FromResult<Invoice?>(null);
        Task<InvoiceItem?> IInvoiceItemService.GetByIdAsync(int id) => Task.FromResult<InvoiceItem?>(null);
        Task<Product?> IProductService.GetByIdAsync(int id) => Task.FromResult<Product?>(null);
        Task<TaxRate?> ITaxRateService.GetByIdAsync(int id) => Task.FromResult<TaxRate?>(null);
        Task<Supplier?> ISupplierService.GetByIdAsync(int id) => Task.FromResult<Supplier?>(null);
        Task<PaymentMethod?> IPaymentMethodService.GetByIdAsync(int id) => Task.FromResult<PaymentMethod?>(null);
        public Task<Invoice?> GetLatestForSupplierAsync(int supplierId) => Task.FromResult<Invoice?>(null);
        public Task<Invoice?> GetLatestAsync() => Task.FromResult<Invoice?>(null);
        public Task SaveAsync(Invoice invoice) => Task.CompletedTask;
        Task IInvoiceItemService.SaveAsync(InvoiceItem item) => Task.CompletedTask;
        Task IProductService.SaveAsync(Product product) => Task.CompletedTask;
        Task ITaxRateService.SaveAsync(TaxRate rate) => Task.CompletedTask;
        Task ISupplierService.SaveAsync(Supplier supplier) => Task.CompletedTask;
        Task IPaymentMethodService.SaveAsync(PaymentMethod method) => Task.CompletedTask;
        public void AddAsync(ChangeLog log) { }
        Task IChangeLogService.AddAsync(ChangeLog log) { AddAsync(log); return Task.CompletedTask; }
        Task<ChangeLog?> IChangeLogService.GetLatestAsync() => Task.FromResult<ChangeLog?>(null);
        public void Pop() { }
        public void PopSubstate() { }
        public void Push(AppState state) { }
        public void PushSubstate(AppState state) { }
        public void SwitchRoot(AppState state) { }
        public bool IsValid(Invoice invoice) => !invoice.HasErrors;

        public IEnumerable<VatSummary> CalculateVatSummary(Invoice invoice)
        {
            if (invoice.Items == null) return Enumerable.Empty<VatSummary>();

            return invoice.Items
                .GroupBy(i => i.TaxRate?.Percentage ?? 0m)
                .Select(g =>
                {
                    decimal net = 0m;
                    decimal vat = 0m;
                    foreach (var item in g)
                    {
                        var amounts = InvoiceApp.Helpers.AmountCalculator.Calculate(
                            item.Quantity,
                            item.UnitPrice,
                            g.Key,
                            invoice.IsGross);
                        net += amounts.Net;
                        vat += amounts.Vat;
                    }
                    return new VatSummary { Rate = g.Key, Net = net, Vat = vat };
                });
        }
    }

    internal static class TestHelpers
    {
        public static InvoiceViewModel CreateInvoiceViewModel()
        {
            var stub = new StubService<object>();
            return new InvoiceViewModel(stub, stub, stub, stub, stub, stub, stub,
                new SupplierViewModel(stub), stub);
        }

        public static ItemsViewModel CreateItemsViewModel(Invoice invoice)
        {
            var stub = new StubService<object>();
            return new ItemsViewModel(
                stub,
                stub,
                stub,
                stub,
                new StatusService(),
                () => { },
                () => { },
                () => invoice.IsGross,
                () => invoice);
        }
    }
}
