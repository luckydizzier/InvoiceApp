using System;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Collections.Generic;
using System.Linq;
using InvoiceApp.Models;
using InvoiceApp.Services;

namespace InvoiceApp.ViewModels
{
    public class InvoiceViewModel : ViewModelBase
    {
        private readonly IInvoiceService _service;
        private readonly IInvoiceItemService _itemService;
        private readonly IProductService _productService;
        private readonly ITaxRateService _taxRateService;
        private readonly ISupplierService _supplierService;
        private readonly IPaymentMethodService _paymentService;
        private readonly IChangeLogService _logService;
        private ObservableCollection<Invoice> _invoices = new();
        private ObservableCollection<InvoiceItemViewModel> _items = new();
        private ObservableCollection<Product> _products = new();
        private ObservableCollection<TaxRate> _taxRates = new();
        private ObservableCollection<Supplier> _suppliers = new();
        private ObservableCollection<PaymentMethod> _paymentMethods = new();
        private Invoice? _selectedInvoice;
        private string _statusMessage = string.Empty;

        public ObservableCollection<Invoice> Invoices
        {
            get => _invoices;
            set
            {
                _invoices = value;
                OnPropertyChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                Items = value != null
                    ? new ObservableCollection<InvoiceItemViewModel>(
                        value.Items.Select(i => new InvoiceItemViewModel(i)))
                    : new ObservableCollection<InvoiceItemViewModel>();
                SelectedSupplier = value?.Supplier;
                SelectedPaymentMethod = value?.PaymentMethod;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<InvoiceItemViewModel> Items
        {
            get => _items;
            set
            {
                _items = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<TaxRate> TaxRates
        {
            get => _taxRates;
            set { _taxRates = value; OnPropertyChanged(); }
        }

        public ObservableCollection<Supplier> Suppliers
        {
            get => _suppliers;
            set { _suppliers = value; OnPropertyChanged(); }
        }

        public ObservableCollection<PaymentMethod> PaymentMethods
        {
            get => _paymentMethods;
            set { _paymentMethods = value; OnPropertyChanged(); }
        }

        public Supplier? SelectedSupplier
        {
            get => SelectedInvoice?.Supplier;
            set
            {
                if (SelectedInvoice != null && SelectedInvoice.Supplier != value)
                {
                    SelectedInvoice.Supplier = value;
                    SelectedInvoice.SupplierId = value?.Id ?? 0;
                    OnPropertyChanged();
                    SuggestNextNumberAsync();
                }
            }
        }

        public PaymentMethod? SelectedPaymentMethod
        {
            get => SelectedInvoice?.PaymentMethod;
            set
            {
                if (SelectedInvoice != null && SelectedInvoice.PaymentMethod != value)
                {
                    SelectedInvoice.PaymentMethod = value;
                    SelectedInvoice.PaymentMethodId = value?.Id ?? 0;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand AddItemCommand { get; }
        public ICommand RemoveItemCommand { get; }
        public ICommand SaveCommand { get; }

        public InvoiceViewModel(IInvoiceService service,
            IInvoiceItemService itemService,
            IProductService productService,
            ITaxRateService taxRateService,
            ISupplierService supplierService,
            IPaymentMethodService paymentService,
            IChangeLogService logService)
        {
            _service = service;
            _itemService = itemService;
            _productService = productService;
            _taxRateService = taxRateService;
            _supplierService = supplierService;
            _paymentService = paymentService;
            _logService = logService;

            AddItemCommand = new RelayCommand(_ => AddItem());
            RemoveItemCommand = new RelayCommand(obj =>
            {
                if (obj is InvoiceItemViewModel item)
                {
                    RemoveItem(item);
                }
            });
            SaveCommand = new RelayCommand(async _ => await SaveAsync());
        }

        public async Task LoadAsync()
        {
            StatusMessage = "Betöltés...";
            var items = await _service.GetAllAsync();
            Invoices = new ObservableCollection<Invoice>(items);

            var prods = await _productService.GetAllAsync();
            Products = new ObservableCollection<Product>(prods);

            var rates = await _taxRateService.GetAllAsync();
            TaxRates = new ObservableCollection<TaxRate>(rates);

            var sups = await _supplierService.GetAllAsync();
            Suppliers = new ObservableCollection<Supplier>(sups);

            var pays = await _paymentService.GetAllAsync();
            PaymentMethods = new ObservableCollection<PaymentMethod>(pays);

            var log = await _logService.GetLatestAsync();
            if (log != null)
            {
                StatusMessage = $"Utolsó esemény: {log.Operation} ({log.DateCreated:g})";
            }
            else
            {
                StatusMessage = Invoices.Count == 0 ? "Üres lista." : $"{Invoices.Count} számla betöltve.";
            }
        }

        private void AddItem()
        {
            if (SelectedInvoice == null) return;
            var firstProduct = Products.FirstOrDefault();
            var firstRate = TaxRates.FirstOrDefault();
            var newItem = new InvoiceItem
            {
                InvoiceId = SelectedInvoice.Id,
                Quantity = 1,
                Product = firstProduct,
                ProductId = firstProduct?.Id ?? 0,
                TaxRate = firstRate,
                TaxRateId = firstRate?.Id ?? 0
            };
            Items.Add(new InvoiceItemViewModel(newItem));
        }

        private void RemoveItem(InvoiceItemViewModel item)
        {
            Items.Remove(item);
        }

        private async void SuggestNextNumberAsync()
        {
            if (SelectedInvoice == null || SelectedInvoice.Id != 0 || SelectedInvoice.SupplierId == 0)
            {
                return;
            }

            var latest = await _service.GetLatestForSupplierAsync(SelectedInvoice.SupplierId);
            if (latest != null)
            {
                SelectedInvoice.Number = IncrementNumber(latest.Number);
                OnPropertyChanged(nameof(SelectedInvoice));
            }
        }

        private static string IncrementNumber(string lastNumber)
        {
            if (string.IsNullOrWhiteSpace(lastNumber)) return "1";

            var digits = new string(lastNumber.Reverse().TakeWhile(char.IsDigit).Reverse().ToArray());
            if (digits.Length > 0 && int.TryParse(digits, out var n))
            {
                var prefix = lastNumber.Substring(0, lastNumber.Length - digits.Length);
                return prefix + (n + 1).ToString($"D{digits.Length}");
            }

            if (int.TryParse(lastNumber, out var value))
            {
                return (value + 1).ToString();
            }

            return lastNumber;
        }

        private async Task SaveAsync()
        {
            if (SelectedInvoice == null) return;

            SelectedInvoice.Items = Items.Select(vm => vm.Item).ToList();
            await _service.SaveAsync(SelectedInvoice);

            foreach (var vm in Items)
            {
                var it = vm.Item;
                if (it.Product != null)
                {
                    await _productService.SaveAsync(it.Product);
                }
                await _itemService.SaveAsync(it);
            }

            StatusMessage = $"Számla mentve. ({DateTime.Now:g})";
        }
    }
}
