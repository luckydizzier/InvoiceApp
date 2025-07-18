using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class DashboardViewModel : ViewModelBase
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IProductService _productService;
        private readonly ISupplierService _supplierService;
        private readonly MainViewModel _main;

        private int _supplierCount;
        private int _invoiceCount;
        private int _productCount;

        public int SupplierCount { get => _supplierCount; private set { _supplierCount = value; OnPropertyChanged(); } }
        public int InvoiceCount { get => _invoiceCount; private set { _invoiceCount = value; OnPropertyChanged(); } }
        public int ProductCount { get => _productCount; private set { _productCount = value; OnPropertyChanged(); } }

        public ObservableCollection<DashboardMenuItem> MenuItems { get; }

        public DashboardViewModel(IInvoiceService invoiceService,
                                  IProductService productService,
                                  ISupplierService supplierService,
                                  MainViewModel main)
        {
            _invoiceService = invoiceService;
            _productService = productService;
            _supplierService = supplierService;
            _main = main;

            MenuItems = new ObservableCollection<DashboardMenuItem>
            {
                new DashboardMenuItem("F1", "Vezérlőpult", main.ShowDashboardCommand),
                new DashboardMenuItem("F4", "Termékek", main.ShowProductsCommand),
                new DashboardMenuItem("F5", "Termékcsoportok", main.ShowProductGroupsCommand),
                new DashboardMenuItem("F6", "Szállítók", main.ShowSuppliersCommand),
                new DashboardMenuItem("F7", "Áfakulcsok", main.ShowTaxRatesCommand),
                new DashboardMenuItem("F8", "Fizetési módok", main.ShowPaymentMethodsCommand)
            };
        }

        public async Task LoadAsync()
        {
            var invoices = await _invoiceService.GetAllAsync();
            var products = await _productService.GetAllAsync();
            var suppliers = await _supplierService.GetAllAsync();
            InvoiceCount = invoices.Count();
            ProductCount = products.Count();
            SupplierCount = suppliers.Count();
        }
    }
}
